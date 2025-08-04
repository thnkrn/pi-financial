using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.RateLimiting;
using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.SetMarketData.DataProcessingService.Helpers;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Helpers;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace Pi.SetMarketData.DataProcessingService.Services;

public sealed class KafkaRecoveryService : BackgroundService, IHealthCheck
{
    private const int MaxProcessingFailures = 100000;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly IConfiguration _config;
    private readonly ConcurrentDictionary<string, int> _failedTopicProcessingCount = new();
    private readonly SemaphoreSlim _healthLock = new(1, 1);
    private readonly ILogger<KafkaRecoveryService> _logger;
    private readonly IKafkaMessageRecovery<Message<string, string>> _messageRecovery;
    private readonly IKafkaOffsetManager _offsetManager;
    private readonly RateLimiter _rateLimiter;
    private readonly DateTime _recoveryEndTime;
    private readonly DateTimeOffset _recoveryStartTime;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly List<string> _topics;
    private IConsumer<string, string>? _consumer;
    private HealthStatus _healthStatus = HealthStatus.Healthy;
    private string _healthStatusReason = string.Empty;

    public KafkaRecoveryService(
        ILogger<KafkaRecoveryService> logger,
        IConfiguration config,
        IKafkaMessageRecovery<Message<string, string>> messageRecovery,
        IKafkaOffsetManager offsetManager)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageRecovery = messageRecovery ?? throw new ArgumentNullException(nameof(messageRecovery));
        _offsetManager = offsetManager ?? throw new ArgumentNullException(nameof(offsetManager));

        _topics = ConfigurationHelper.GetTopicList(_config, ConfigurationKeys.KafkaTopic);

        // Get recovery dates from config or use defaults
        var startDate = _config["Recovery:StartDate"] ?? "2025-04-15T00:00:00Z";
        var endDate = _config["Recovery:EndDate"] ?? "2025-04-19T23:59:59Z";

        _recoveryStartTime = DateTimeOffset.Parse(startDate, new DateTimeFormatInfo());
        _recoveryEndTime = DateTimeOffset.Parse(endDate, new DateTimeFormatInfo()).UtcDateTime;

        // Configure rate limiter to prevent overwhelming downstream systems
        _rateLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = int.Parse(_config["RateLimits:TokenLimit"] ?? "15000"),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = int.Parse(_config["RateLimits:QueueLimit"] ?? "8000"),
            ReplenishmentPeriod =
                TimeSpan.FromSeconds(double.Parse(_config["RateLimits:ReplenishmentPeriodSeconds"] ?? "1")),
            TokensPerPeriod = int.Parse(_config["RateLimits:TokensPerPeriod"] ?? "15"),
            AutoReplenishment = true
        });

        // Configure retry policy
        _retryPolicy = Policy
            .Handle<Exception>(ex => ex is not OperationCanceledException)
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromSeconds(Math.Pow(1.8, attempt)),
                (ex, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(ex,
                        "Error during processing message. Retry {RetryCount} after {RetryTimeSpan}ms",
                        retryCount, timeSpan.TotalMilliseconds);
                });

        // Configure circuit breaker to prevent system overload
        _circuitBreaker = Policy
            .Handle<Exception>(ex => ex is not OperationCanceledException)
            .CircuitBreakerAsync(
                100000,
                TimeSpan.FromSeconds(45),
                (ex, breakDuration) =>
                {
                    _logger.LogError(ex,
                        "Circuit breaker opened for {BreakDuration}. Service will pause processing",
                        breakDuration);
                    UpdateHealthStatus(HealthStatus.Unhealthy, $"Circuit breaker opened: {ex.Message}");
                },
                () =>
                {
                    _logger.LogDebug("Circuit breaker reset. Service resuming normal operation");
                    UpdateHealthStatus(HealthStatus.Healthy, string.Empty);
                });
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        await _healthLock.WaitAsync(cancellationToken);
        try
        {
            return _healthStatus switch
            {
                HealthStatus.Healthy => HealthCheckResult.Healthy(),
                HealthStatus.Degraded => HealthCheckResult.Degraded(_healthStatusReason),
                _ => HealthCheckResult.Unhealthy(_healthStatusReason)
            };
        }
        finally
        {
            _healthLock.Release();
        }
    }

    private void UpdateHealthStatus(HealthStatus status, string reason)
    {
        _healthLock.Wait();
        try
        {
            _healthStatus = status;
            _healthStatusReason = reason;
        }
        finally
        {
            _healthLock.Release();
        }
    }

    private ConsumerConfig CreateConsumerConfig()
    {
        _logger.LogDebug("Creating Kafka consumer configuration");
        var config = new ConsumerConfig
        {
            BootstrapServers = _config[ConfigurationKeys.KafkaBootstrapServers],
            GroupId = _config["Kafka:ConsumerGroupId"] ?? "set_data_recovery_client103",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnablePartitionEof = true,
            // Add additional consumer configuration for better performance and resilience
            SessionTimeoutMs = int.Parse(_config["Kafka:SessionTimeoutMs"] ?? "40000"),
            HeartbeatIntervalMs = int.Parse(_config["Kafka:HeartbeatIntervalMs"] ?? "13000"),
            MaxPollIntervalMs = int.Parse(_config["Kafka:MaxPollIntervalMs"] ?? "400000"),
            FetchMaxBytes = int.Parse(_config["Kafka:FetchMaxBytes"] ?? "52428800"),
            MessageMaxBytes = int.Parse(_config["Kafka:MessageMaxBytes"] ?? "13107200"),
            FetchWaitMaxMs = int.Parse(_config["Kafka:FetchWaitMaxMs"] ?? "300"),
            QueuedMinMessages = int.Parse(_config["Kafka:QueuedMinMessages"] ?? "3000"),
            SocketTimeoutMs = int.Parse(_config["Kafka:SocketTimeoutMs"] ?? "75000")
        };

        try
        {
            var securityProtocolString = _config[ConfigurationKeys.KafkaSecurityProtocol] ?? "SASL_SSL";
            config.SecurityProtocol = Enum.TryParse<SecurityProtocol>(
                securityProtocolString.Replace("_", string.Empty), true, out var securityProtocol)
                ? securityProtocol
                : SecurityProtocol.SaslSsl;

            var saslMechanismString = _config[ConfigurationKeys.KafkaSaslMechanism] ?? "PLAIN";
            config.SaslMechanism = Enum.TryParse<SaslMechanism>(
                saslMechanismString.Replace("_", string.Empty), true, out var saslMechanism)
                ? saslMechanism
                : SaslMechanism.Plain;

            config.SaslUsername = _config[ConfigurationKeys.KafkaSaslUsername] ??
                                  throw new InvalidOperationException(
                                      $"Missing required configuration: {ConfigurationKeys.KafkaSaslUsername}");

            config.SaslPassword = _config[ConfigurationKeys.KafkaSaslPassword] ??
                                  throw new InvalidOperationException(
                                      $"Missing required configuration: {ConfigurationKeys.KafkaSaslPassword}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error configuring Kafka security settings");
            throw new InvalidOperationException("Failed to configure Kafka security settings", ex);
        }

        return config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_topics.Count == 0)
        {
            _logger.LogError("No valid topic configured for recovery. Service will not start.");
            UpdateHealthStatus(HealthStatus.Unhealthy, "No valid topics configured");
            return;
        }

        _logger.LogInformation("Starting Kafka recovery service for {TopicCount} topics", _topics.Count);

        try
        {
            // Create consumer once, outside the loop
            var consumerConfig = CreateConsumerConfig();
            _consumer = new ConsumerBuilder<string, string>(consumerConfig)
                .SetErrorHandler((_, error) =>
                {
                    _logger.LogError("Kafka consumer error: {ErrorReason} ({ErrorCode})", error.Reason, error.Code);
                })
                .SetStatisticsHandler((_, stats) =>
                {
                    // Log consumer stats periodically
                    _logger.LogDebug("Kafka consumer stats: {Stats}", stats);
                })
                .Build();

            await ProcessTopicsAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Nothing to do
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in Kafka recovery service");
            UpdateHealthStatus(HealthStatus.Unhealthy, $"Service terminated with error: {ex.Message}");
            throw new InvalidOperationException(ex.Message);
        }
        finally
        {
            try
            {
                _consumer?.Close();
                _consumer?.Dispose();
                _logger.LogDebug("Kafka consumer closed and disposed");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during Kafka consumer cleanup");
            }
        }
    }

    private async Task ProcessTopicsAsync(CancellationToken stoppingToken)
    {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var recoveryTopic in _topics)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            _logger.LogInformation(
                "Starting Kafka recovery for topic {Topic} from {RecoveryStart} UTC to {RecoveryEnd} UTC",
                recoveryTopic, _recoveryStartTime, _recoveryEndTime);

            // Reset failure counter for this topic
            _failedTopicProcessingCount[recoveryTopic] = 0;

            try
            {
                await ProcessTopicAsync(recoveryTopic, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error processing topic {Topic}. Moving to next topic.", recoveryTopic);
                UpdateHealthStatus(HealthStatus.Degraded, $"Error processing topic {recoveryTopic}: {ex.Message}");
            }
        }

        _logger.LogInformation("Kafka recovery service completed for all topics");
    }

    private async Task ProcessTopicAsync(string recoveryTopic, CancellationToken stoppingToken)
    {
        using var adminClient = new AdminClientBuilder(CreateConsumerConfig()).Build();

        // Get topic metadata with timeout
        var metadata = await GetTopicMetadataAsync(adminClient, recoveryTopic, stoppingToken);
        var partitions = metadata?.Topics
            .First(t => t.Topic == recoveryTopic)
            .Partitions
            .Select(p => new TopicPartition(recoveryTopic, new Partition(p.PartitionId)))
            .ToList();

        if (partitions == null || partitions.Count == 0)
        {
            _logger.LogWarning("No partitions found for topic {Topic}", recoveryTopic);
            return;
        }

        _logger.LogInformation("Found {PartitionCount} partitions for topic {Topic}",
            partitions.Count, recoveryTopic);

        var assignmentsToUse = new List<TopicPartitionOffset>();

        foreach (var partition in partitions)
        {
            var savedOffset = await _offsetManager.GetLastCommittedOffsetAsync(
                partition.Topic, partition.Partition.Value);

            if (savedOffset != null)
            {
                var isWithinTimeRange = _consumer != null && await _offsetManager.IsOffsetWithinTimeRangeAsync(
                    partition.Topic, partition.Partition.Value, savedOffset.Offset.Value, _consumer);

                if (isWithinTimeRange)
                {
                    assignmentsToUse.Add(new TopicPartitionOffset(
                        partition,
                        new Offset(savedOffset.Offset.Value + 1)));

                    _logger.LogInformation("Using stored offset {Offset} for {Partition} within time range",
                        savedOffset.Offset.Value + 1, partition);
                }
                else
                {
                    await AssignUsingTimestampAsync(partition, assignmentsToUse, stoppingToken);
                }
            }
            else
            {
                await AssignUsingTimestampAsync(partition, assignmentsToUse, stoppingToken);
            }
        }

        if (assignmentsToUse.Count == 0)
        {
            _logger.LogWarning("No valid offsets found for any partition of topic {Topic}", recoveryTopic);
            return;
        }

        // Assign the consumer to the partitions with specific offsets
        _logger.LogInformation("Assigning consumer to {Count} partitions with specific offsets",
            assignmentsToUse.Count);
        _consumer?.Assign(assignmentsToUse);

        await ConsumeMessagesAsync(recoveryTopic, stoppingToken);
    }

    private async Task AssignUsingTimestampAsync(
        TopicPartition partition,
        List<TopicPartitionOffset> assignmentsToUse,
        CancellationToken stoppingToken)
    {
        var timestampRequest = new TopicPartitionTimestamp(
            partition, new Timestamp(_recoveryStartTime.UtcDateTime));

        // Get offsets with timeout
        var offset = await GetOffsetForTimestampAsync(timestampRequest, stoppingToken);

        if (offset != null && offset.Offset != Offset.Unset)
        {
            assignmentsToUse.Add(offset);
            _logger.LogDebug("Using timestamp-based offset {Offset} for {Partition}",
                offset.Offset, partition);
        }
        else
        {
            _logger.LogWarning("No valid offset found for {Partition} at timestamp {Timestamp}",
                partition, _recoveryStartTime);
        }
    }

    private async Task<Metadata?> GetTopicMetadataAsync(IAdminClient adminClient, string topic,
        CancellationToken stoppingToken)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(() =>
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                cts.CancelAfter(TimeSpan.FromSeconds(30)); // Timeout after 30 seconds

                try
                {
                    return Task.FromResult(adminClient.GetMetadata(topic, TimeSpan.FromSeconds(10)));
                }
                catch (OperationCanceledException) when (cts.IsCancellationRequested &&
                                                         !stoppingToken.IsCancellationRequested)
                {
                    throw new TimeoutException($"Timeout while getting metadata for topic {topic}");
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get metadata for topic {Topic}", topic);
            return null;
        }
    }

    private async Task<TopicPartitionOffset?> GetOffsetForTimestampAsync(
        TopicPartitionTimestamp timestampRequest,
        CancellationToken stoppingToken)
    {
        if (_consumer == null) return null;

        try
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                cts.CancelAfter(TimeSpan.FromSeconds(30)); // Timeout after 30 seconds

                try
                {
                    await Task.Yield();
                    var offsets = _consumer.OffsetsForTimes([timestampRequest], TimeSpan.FromSeconds(10));
                    return offsets.FirstOrDefault();
                }
                catch (OperationCanceledException) when (cts.IsCancellationRequested &&
                                                         !stoppingToken.IsCancellationRequested)
                {
                    throw new TimeoutException($"Timeout getting offset for timestamp {timestampRequest.Timestamp}");
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting offset for {Partition} at timestamp {Timestamp}",
                timestampRequest.TopicPartition, timestampRequest.Timestamp);
            return null;
        }
    }

    private async Task ConsumeMessagesAsync(string recoveryTopic, CancellationToken stoppingToken)
    {
        var messageCount = 0;
        var successCount = 0;
        var failureCount = 0;
        var lastProgress = DateTime.UtcNow;
        var batchStartTime = DateTime.UtcNow;
        var currentBatchSize = 0;
        const int batchReportSize = 3000;

        _logger.LogInformation("Starting message consumption for topic {Topic}", recoveryTopic);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_consumer?.Assignment.Count == 0)
            {
                _logger.LogInformation("No more partitions assigned for topic {Topic}. Consumption complete.",
                    recoveryTopic);
                break;
            }

            ConsumeResult<string, string>? result;

            try
            {
                // Apply rate limiting
                using var lease = await _rateLimiter.AcquireAsync(1, stoppingToken);
                if (lease.IsAcquired)
                {
                    // Execute with circuit breaker and retry policies
                    result = await _circuitBreaker.ExecuteAsync(async () =>
                    {
                        return await Task.Run(() => _consumer?.Consume(TimeSpan.FromSeconds(1)), stoppingToken);
                    });
                }
                else
                {
                    _logger.LogWarning("Rate limit exceeded. Slowing down consumption.");
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                    continue;
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error consuming messages from Kafka");

                // Increase the failure count for this topic
                IncrementFailureCount(recoveryTopic);

                if (ShouldStopProcessingTopic(recoveryTopic))
                {
                    _logger.LogError("Too many failures for topic {Topic}. Stopping processing of this topic.",
                        recoveryTopic);
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                continue;
            }

            if (result == null) continue;

            if (result.IsPartitionEOF)
            {
                _logger.LogDebug("Reached end of partition {Partition} for topic {Topic}",
                    result.Partition, recoveryTopic);
                continue;
            }

            if (result.Message.Timestamp.UtcDateTime < _recoveryStartTime.UtcDateTime)
            {
                _logger.LogDebug(
                    "Skipping message with timestamp {MessageTime} before recovery start time {StartTime}",
                    result.Message.Timestamp.UtcDateTime, _recoveryStartTime.UtcDateTime);
                continue;
            }

            // Check if we've reached the end time cutoff
            if (result.Message.Timestamp.UtcDateTime > _recoveryEndTime)
            {
                _logger.LogInformation(
                    "Reached cutoff timestamp {EndTime} for recovery on partition {Partition}",
                    _recoveryEndTime, result.Partition);

                // Remove this partition from assignment since we're done with it
                var remainingAssignments = _consumer?.Assignment
                    .Where(tp => !tp.Equals(result.TopicPartition))
                    .ToList();

                if (remainingAssignments?.Count > 0)
                    _consumer?.Assign(remainingAssignments);
                else
                    _consumer?.Unassign();

                continue;
            }

            messageCount++;
            currentBatchSize++;

            // Periodic progress reporting
            var shouldReportProgress = messageCount % batchReportSize == 0 ||
                                       (DateTime.UtcNow - lastProgress).TotalSeconds > 30;

            if (shouldReportProgress)
            {
                var elapsedTime = (DateTime.UtcNow - batchStartTime).TotalSeconds;
                var messagesPerSecond = elapsedTime > 0 ? currentBatchSize / elapsedTime : 0;

                _logger.LogInformation(
                    "Progress for topic {Topic}: processed {MessageCount} messages total " +
                    "({SuccessCount} successful, {FailureCount} failed, {MessagesPerSecond:F1} messages/sec)",
                    recoveryTopic, messageCount, successCount, failureCount, messagesPerSecond);

                lastProgress = DateTime.UtcNow;
                batchStartTime = DateTime.UtcNow;
                currentBatchSize = 0;
            }

            try
            {
                await ProcessMessageAsync(result, recoveryTopic);
                successCount++;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                failureCount++;

                IncrementFailureCount(recoveryTopic);

                if (ShouldStopProcessingTopic(recoveryTopic))
                {
                    _logger.LogError(ex, "Too many failures for topic {Topic}. Stopping processing of this topic.",
                        recoveryTopic);
                    break;
                }
            }
        }

        _logger.LogInformation(
            "Recovery complete for topic {Topic}: processed {MessageCount} messages, {SuccessCount} successful, {FailureCount} failed",
            recoveryTopic, messageCount, successCount, failureCount);
    }

    private async Task ProcessMessageAsync(ConsumeResult<string, string>? result, string topic)
    {
        try
        {
            var handled = await _retryPolicy.ExecuteAsync(async () => result?.Message != null
                                                                      && await _messageRecovery.HandleAsync(
                                                                          result.Message));
            if (handled)
                try
                {
                    // Execute commit with retry policy
                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        await Task.Run(() => _consumer?.Commit(result));

                        if (result != null)
                            await _offsetManager.SaveOffsetAsync(
                                result.Topic,
                                result.Partition.Value,
                                result.Offset.Value);

                        return true;
                    });

                    if (result != null)
                        _logger.LogDebug("Committed offset {Offset} for partition {Partition}",
                            result.Offset, result.Partition);
                }
                catch (Exception ex)
                {
                    if (result != null)
                        _logger.LogWarning(ex,
                            "Failed to commit offset {Offset} for topic {Topic}, partition {Partition}",
                            result.Offset, topic, result.Partition);
                    throw;
                }
        }
        catch (Exception ex)
        {
            if (result != null)
                _logger.LogError(ex, "Error while handling message at offset {Offset} for topic {Topic}",
                    result.Offset, topic);
            throw new InvalidOperationException(ex.Message);
        }
    }

    private void IncrementFailureCount(string topic)
    {
        _failedTopicProcessingCount.AddOrUpdate(
            topic,
            1,
            (_, count) => count + 1);
    }

    private bool ShouldStopProcessingTopic(string topic)
    {
        return _failedTopicProcessingCount.TryGetValue(topic, out var count) &&
               count >= MaxProcessingFailures;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Kafka recovery service");

        try
        {
            _consumer?.Close();
            _consumer?.Dispose();
            _consumer = null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during Kafka consumer cleanup");
        }

        await base.StopAsync(cancellationToken);
        _logger.LogInformation("Kafka recovery service stopped");
    }
}