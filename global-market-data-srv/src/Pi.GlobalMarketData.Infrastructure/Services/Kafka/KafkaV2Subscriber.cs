using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks.Dataflow;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;

namespace Pi.GlobalMarketData.Infrastructure.Services.Kafka;

public sealed class KafkaV2Subscriber<TKey, TValue> : IKafkaV2Subscriber, IDisposable where TKey : notnull
{
    private readonly TimeSpan _batchInterval;
    private readonly int _batchSize;
    private readonly int _boundedCapacity;
    private readonly SemaphoreSlim _circuitBreakerLock = new(1, 1);
    private readonly SemaphoreSlim _commitLock = new(1, 1);
    private readonly Timer _commitTimer;

    // Configuration and state variables
    private readonly IConfiguration _configuration;
    private readonly TransformBlock<ConsumeResult<TKey, TValue>, ConsumeResult<TKey, TValue>> _preprocessBlock;
    private readonly CancellationTokenSource _internalCts = new();
    private readonly ILogger<KafkaV2Subscriber<TKey, TValue>> _logger;
    private readonly int _maxConsecutiveErrors;
    private readonly Timer _metricsTimer;

    // Data structures for tracking processing
    private readonly ConcurrentDictionary<TopicPartition, Offset> _pendingOffsets = new();

    // Store processed messages waiting to be committed
    private readonly ConcurrentDictionary<TopicPartitionOffset, bool> _processedMessages = new();
    private readonly ActionBlock<ConsumeResult<TKey, TValue>> _processingBlock;
    private readonly List<string> _topic;
    private int _consecutiveErrors;
    private IConsumer<TKey, TValue>? _consumer;
    private bool _disposed;
    private long _lastCommitTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    private long _lastMetricsTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private volatile bool _locksDisposed;
    private volatile bool _reBalanceInProgress;
    private volatile bool _shuttingDown;
    private long _totalCommitAttempts;
    private long _totalCommitFailures;

    private long _totalFailedMessages;
    private long _totalMessagesProcessed;

    // Processing metrics 
    private long _totalMessagesReceived;
    private long _totalSkippedMessages;

    [SuppressMessage("SonarQube", "S3776")]
    public KafkaV2Subscriber(IConfiguration configuration,
        List<string> topic,
        IKafkaMessageV2Handler<Message<TKey, TValue>> messageHandler,
        ILogger<KafkaV2Subscriber<TKey, TValue>> logger)
    {
        _logger = logger;
        _configuration = configuration;
        _topic = topic;

        // Performance configuration settings
        var maxConcurrentTasks = _configuration.GetValue("KAFKA:CONSUMER_TUNED_MAX_CONCURRENT_TASKS", 100);
        _batchSize = _configuration.GetValue("KAFKA:CONSUMER_TUNED_MAX_BATCH_SIZE", 15);
        _batchInterval =
            TimeSpan.FromMilliseconds(_configuration.GetValue("KAFKA:CONSUMER_TUNED_BATCH_INTERVAL_MS", 100));
        _maxConsecutiveErrors = _configuration.GetValue("KAFKA:CONSUMER_TUNED_MAX_CONSECUTIVE_ERRORS", 10);
        _boundedCapacity = _configuration.GetValue("KAFKA:CONSUMER_TUNED_MAX_BOUNDED_CAPACITY", 100000);

        // Commit timer setup
        var commitInterval =
            TimeSpan.FromSeconds(_configuration.GetValue("KAFKA:CONSUMER_TUNED_COMMIT_INTERVAL_SECONDS", 3));
        _commitTimer = new Timer(CommitPendingOffsets, null, commitInterval, commitInterval);

        // Metrics timer setup
        var metricsInterval =
            TimeSpan.FromSeconds(_configuration.GetValue("KAFKA:CONSUMER_TUNED_METRICS_INTERVAL_SECONDS", 60));
        _metricsTimer = new Timer(LogPerformanceMetrics, null, metricsInterval, metricsInterval);

        // Create and link DataFlow blocks
        var dataflowOptions = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = _boundedCapacity,
            // Set 1 for preprocessing is sequential to maintain order
            MaxDegreeOfParallelism = 1,
            CancellationToken = _internalCts.Token
        };

        // Block for initial preprocessing
        _preprocessBlock = new TransformBlock<ConsumeResult<TKey, TValue>, ConsumeResult<TKey, TValue>>(
            Task.FromResult, // Simplified inline preprocessing
            dataflowOptions);

        // Block for message processing
        _processingBlock = new ActionBlock<ConsumeResult<TKey, TValue>>(
            async message =>
            {
                try
                {
                    try
                    {
                        // Process the message with updated interface
                        // Only mark as processed and track offset if successful
                        var success = await messageHandler.HandleAsync(message.Message).ConfigureAwait(false);
                        if (success)
                        {
                            // Update metrics
                            Interlocked.Increment(ref _totalMessagesProcessed);

                            // Mark message as successfully processed
                            var tpo = new TopicPartitionOffset(message.TopicPartition, message.Offset + 1);
                            _processedMessages.TryAdd(tpo, true);

                            // Track offset after successful processing
                            TrackOffset(message);
                        }
                        else
                        {
                            // Track failed messages for monitoring
                            // Don't track offset for failed messages
                            // This way they'll be reprocessed if consumer restarts
                            Interlocked.Increment(ref _totalFailedMessages);
                            _logger.LogWarning(
                                "Message processing returned failure for Topic={Topic}, Partition={Partition}, Offset={Offset}",
                                message.Topic, message.Partition, message.Offset);

                            var msg = message.Message.Value?.ToString();
                            _logger.LogInformation("Message failure: {Msg}", msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message: {Message}", ex.Message);
                        Interlocked.Increment(ref _totalFailedMessages);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error in message processing pipeline: {Message}", ex.Message);
                    Interlocked.Increment(ref _totalFailedMessages);

                    // Rethrow only critical errors that should crash the processing
                    if (ex is OutOfMemoryException or StackOverflowException or ThreadAbortException)
                        throw;
                }
            },
            new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = dataflowOptions.BoundedCapacity,
                MaxDegreeOfParallelism = maxConcurrentTasks,
                CancellationToken = _internalCts.Token
            });

        // Link the blocks to create the pipeline
        _preprocessBlock.LinkTo(_processingBlock, new DataflowLinkOptions { PropagateCompletion = true });

        _logger.LogInformation(
            "Initialized TPL Dataflow pipeline with concurrency: {MaxConcurrency}, batch size: {BatchSize}",
            maxConcurrentTasks, _batchSize);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task SubscribeAsync(CancellationToken cancellationToken)
    {
        _shuttingDown = false;
        _consecutiveErrors = 0;

        var config = CreateConsumerConfig();

        using var linkedTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _internalCts.Token);
        await Task.Factory.StartNew(
            () => HandleSubscribeAsync(config, linkedTokenSource.Token),
            TaskCreationOptions.LongRunning);
    }

    public async Task UnsubscribeAsync()
    {
        _shuttingDown = true;

        await _internalCts.CancelAsync();

        // Complete the pipeline to allow graceful shutdown
        _preprocessBlock.Complete();

        // Try to commit any pending offsets before shutting down
        try
        {
            await CommitPendingOffsetsAsync(true).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error committing offsets during shutdown");
        }

        try
        {
            // Best-effort wait for pipeline to complete
            await Task.WhenAll(
                _preprocessBlock.Completion,
                _processingBlock.Completion
            ).WaitAsync(TimeSpan.FromMinutes(20));
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogDebug(ex, "Tasks were canceled during shutdown as expected");
        }
        catch (TimeoutException ex)
        {
            _logger.LogDebug(ex, "Timeout waiting for message processing to complete");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error waiting for message processing to complete");
        }

        _commitLock.Dispose();
        _circuitBreakerLock.Dispose();
        _internalCts.Dispose();

        try
        {
            // Properly close the consumer to leave the consumer group cleanly
            _consumer?.Close();
            _consumer?.Dispose();
            _consumer = null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error closing Kafka consumer during shutdown");
        }
    }

    private ConsumerConfig CreateConsumerConfig()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration[ConfigurationKeys.KafkaBootstrapServers],
            GroupId = _configuration[ConfigurationKeys.KafkaConsumerGroupId],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,

            // Performance optimization settings
            FetchMinBytes =
                _configuration.GetValue("KAFKA:CONSUMER_TUNED_FETCH_MIN_BYTES", 16384),
            FetchMaxBytes = _configuration.GetValue("KAFKA:CONSUMER_TUNED_FETCH_MAX_BYTES", 10485760),
            FetchWaitMaxMs =
                _configuration.GetValue("KAFKA:CONSUMER_TUNED_FETCH_WAIT_MAX_MS",
                    200), // Increased to balance latency and throughput
            MaxPartitionFetchBytes =
                _configuration.GetValue("KAFKA:CONSUMER_TUNED_MAX_PARTITION_FETCH_BYTES", 10485760),

            // Buffer settings for high throughput
            QueuedMinMessages =
                _configuration.GetValue("KAFKA:CONSUMER_TUNED_QUEUED_MIN_MESSAGES", 20000),
            QueuedMaxMessagesKbytes =
                _configuration.GetValue("KAFKA:CONSUMER_TUNED_QUEUED_MAX_MESSAGES_KBYTES", 524288),

            // Stability settings
            SessionTimeoutMs = _configuration.GetValue("KAFKA:CONSUMER_TUNED_SESSION_TIMEOUT_MS", 90000),
            MaxPollIntervalMs = _configuration.GetValue("KAFKA:CONSUMER_TUNED_MAX_POLL_INTERVAL_MS", 900000),
            HeartbeatIntervalMs = _configuration.GetValue("KAFKA:CONSUMER_TUNED_HEARTBEAT_INTERVAL_MS", 30000),

            // Network settings
            SocketKeepaliveEnable = true,
            SocketTimeoutMs = _configuration.GetValue("KAFKA:CONSUMER_TUNED_SOCKET_TIMEOUT_MS", 60000),
            ConnectionsMaxIdleMs = _configuration.GetValue("KAFKA:CONSUMER_TUNED_CONNECTIONS_MAX_IDLE_MS", 180000),
            ReconnectBackoffMaxMs = _configuration.GetValue("KAFKA:CONSUMER_TUNED_RECONNECT_BACKOFF_MAX_MS", 10000),
            ReconnectBackoffMs = _configuration.GetValue("KAFKA:CONSUMER_TUNED_RECONNECT_BACKOFF_MS", 100),

            // Performance optimization: set to range for more balanced partitioning
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.Range
        };

        try
        {
            // Configure security settings if needed
            var securityProtocolString = _configuration[ConfigurationKeys.KafkaSecurityProtocol] ?? "SASL_SSL";
            config.SecurityProtocol = Enum.TryParse<SecurityProtocol>(
                securityProtocolString.Replace("_", string.Empty), true, out var securityProtocol)
                ? securityProtocol
                : SecurityProtocol.SaslSsl;

            var saslMechanismString = _configuration[ConfigurationKeys.KafkaSaslMechanism] ?? "PLAIN";
            config.SaslMechanism = Enum.TryParse<SaslMechanism>(
                saslMechanismString.Replace("_", string.Empty), true, out var saslMechanism)
                ? saslMechanism
                : SaslMechanism.Plain;

            config.SaslUsername = _configuration[ConfigurationKeys.KafkaSaslUsername];
            config.SaslPassword = _configuration[ConfigurationKeys.KafkaSaslPassword];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error configuring Kafka security settings");
            throw new InvalidOperationException("Failed to configure Kafka security settings", ex);
        }

        return config;
    }

    [SuppressMessage("SonarQube", "S3776")]
    private async Task HandleSubscribeAsync(ConsumerConfig config, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && !_shuttingDown)
            try
            {
                _consumer = new ConsumerBuilder<TKey, TValue>(config)
                    .SetErrorHandler((_, e) => { _logger.LogError("Kafka error: {Reason}", e.Reason); })
                    .SetPartitionsAssignedHandler((_, partitions) =>
                    {
                        _reBalanceInProgress = false;
                        _logger.LogDebug("Partitions assigned: {Partitions}",
                            string.Join(", ", partitions.Select(p => $"{p.Topic}-{p.Partition}")));
                    })
                    .SetPartitionsRevokedHandler((_, partitions) =>
                    {
                        _reBalanceInProgress = true;
                        _logger.LogDebug("Partitions revoked: {Partitions}",
                            string.Join(", ", partitions.Select(p => $"{p.Topic}-{p.Partition}")));

                        try
                        {
                            // ReSharper disable AccessToDisposedClosure
                            if (!_locksDisposed && _consumer != null)
                            {
                                var pendingOffsetsCopy = _pendingOffsets.ToArray();
                                if (pendingOffsetsCopy.Length > 0)
                                {
                                    var offsetsToCommit = pendingOffsetsCopy
                                        .Select(kv => new TopicPartitionOffset(kv.Key, new Offset(kv.Value)))
                                        .ToList();

                                    try
                                    {
                                        _consumer.Commit(offsetsToCommit);
                                        _logger.LogInformation("Committed {Count} offsets during re-balance",
                                            offsetsToCommit.Count);
                                    }
                                    catch (Exception commitEx)
                                    {
                                        _logger.LogDebug(commitEx, "Failed to commit offsets during re-balance");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogDebug(ex, "Error handling re-balance");
                        }
                    })
                    .Build();

                _consumer.Subscribe(_topic);
                _consecutiveErrors = 0;

                // Process messages in batches for high throughput
                await ProcessMessageBatchesAsync(cancellationToken);
                break;
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
                break;
            }
            catch (Exception ex)
            {
                await HandleError(ex, "Error occurred while consuming messages", cancellationToken);

                // Commit pending offsets before disposing to prevent data loss
                try
                {
                    await CommitPendingOffsetsAsync(true).ConfigureAwait(false);
                }
                catch (Exception commitEx)
                {
                    _logger.LogError(commitEx, "Failed to commit offsets before consumer restart");
                }

                // Dispose of the consumer before retrying
                try
                {
                    _consumer?.Close();
                    _consumer?.Dispose();
                    _consumer = null;
                }
                catch
                {
                    // Ignore errors during consumer disposal
                }

                if (!cancellationToken.IsCancellationRequested && !_shuttingDown)
                    await Task.Delay(3000, cancellationToken); // Backoff before retry
            }
    }

    private async Task ProcessMessageBatchesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && !_shuttingDown)
            try
            {
                // Use stopwatch for batch time limiting
                var batchStopwatch = Stopwatch.StartNew();
                var batch = await ConsumeResults(cancellationToken, batchStopwatch);

                // Process the collected batch
                await ProcessCollectedBatch(cancellationToken, batch);

                // Trigger periodic commits based on batch size rather than just timer
                // This helps ensure more timely commits based on actual throughput
                if (batch.Count >= _batchSize / 2)
                    // Not awaiting to avoid blocking the processing loop
                    // But still schedule more frequent commits after processing batches
                    _ = Task.Run(() => CommitPendingOffsetsAsync(false).ConfigureAwait(false), cancellationToken)
                        .ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                                _logger.LogError(t.Exception, "Error during background commit after batch");
                        }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                await HandleError(ex, "Error in batch message consumption loop", cancellationToken);
            }
    }

    [SuppressMessage("SonarQube", "S3776")]
    private async Task ProcessCollectedBatch(CancellationToken cancellationToken,
        List<ConsumeResult<TKey, TValue>> batch)
    {
        if (batch.Count > 0)
        {
            _logger.LogDebug("Processing batch of {Count} messages", batch.Count);

            // Implement back-pressure when buffer is filling up
            if (_preprocessBlock.InputCount > _boundedCapacity * 0.8)
            {
                _logger.LogWarning("Buffer is filling up ({CurrentCount}/{MaxCapacity}). Slowing down consumption.",
                    _preprocessBlock.InputCount, _boundedCapacity);

                // Exponential backoff based on buffer fullness
                var bufferFullnessRatio = (double)_preprocessBlock.InputCount / _boundedCapacity;
                var delayMs = (int)(100 * Math.Min(10, Math.Pow(1.5, bufferFullnessRatio * 10)));

                await Task.Delay(delayMs, cancellationToken);
            }

            const int maxAttempts = 10;
            const int baseDelay = 10;

            foreach (var message in batch)
            {
                var posted = _preprocessBlock.Post(message);
                if (!posted)
                {
                    var attempts = 0;

                    while (!posted && attempts < maxAttempts && !cancellationToken.IsCancellationRequested)
                    {
                        attempts++;
                        // Exponential backoff
                        var delay = baseDelay * (1 << Math.Min(8, attempts));

                        _logger.LogDebug("Preprocessing block full, attempt {Attempt}/{MaxAttempts}, waiting {Delay}ms",
                            attempts, maxAttempts, delay);

                        await Task.Delay(delay, cancellationToken);
                        posted = _preprocessBlock.Post(message);
                    }

                    if (!posted && !cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogWarning(
                            "Unable to post message to preprocessing block after {Attempts} attempts - skipping but NOT tracking offset",
                            attempts);

                        // IMPORTANT: We do NOT track the offset for messages we couldn't process.
                        // These will be reprocessed after consumer restarts or re-balances.
                        Interlocked.Increment(ref _totalSkippedMessages);
                    }
                }
            }
        }
    }

    private async Task<List<ConsumeResult<TKey, TValue>>> ConsumeResults(CancellationToken cancellationToken,
        Stopwatch batchStopwatch)
    {
        var batch = new List<ConsumeResult<TKey, TValue>>(_batchSize);

        // Collect batch of messages until we reach size or time limit
        var messagesInBatch = 0;
        while (messagesInBatch < _batchSize &&
               batchStopwatch.ElapsedMilliseconds < _batchInterval.TotalMilliseconds &&
               !cancellationToken.IsCancellationRequested)
            try
            {
                var consumeResult = _consumer?.Consume(TimeSpan.FromMilliseconds(10));
                if (consumeResult != null && !Equals(consumeResult.Message.Value, default(TValue)))
                {
                    Interlocked.Increment(ref _totalMessagesReceived);
                    batch.Add(consumeResult);
                    messagesInBatch++;
                }
                else if (batch.Count == 0)
                {
                    // Prevent CPU spinning when no messages available
                    await Task.Delay(10, cancellationToken);
                    break;
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Consume error: {ErrorReason}", ex.Error.Reason);
            }

        return batch;
    }

    private void TrackOffset(ConsumeResult<TKey, TValue> consumeResult)
    {
        // Only track the highest processed offset for each partition
        _pendingOffsets.AddOrUpdate(
            consumeResult.TopicPartition,
            consumeResult.Offset + 1,
            (_, existingOffset) => Math.Max(existingOffset.Value, consumeResult.Offset.Value + 1));
    }

    private void CommitPendingOffsets(object? state)
    {
        // Use the async version but don't await it to avoid blocking the timer
        _ = CommitPendingOffsetsAsync(false);
    }

    [SuppressMessage("SonarQube", "S3776")]
    private async Task<bool> CommitPendingOffsetsAsync(bool isShutdown)
    {
        if (_consumer == null || _disposed || (_shuttingDown && !isShutdown) || _locksDisposed)
            return false;

        // If there's a re-balance in progress and this is not a shutdown operation, skip
        if (_reBalanceInProgress && !isShutdown)
            return false;

        try
        {
            // Use a lock to ensure only one commit happens at a time
            if (!await _commitLock.WaitAsync(isShutdown ? 30000 : 1000))
            {
                _logger.LogWarning("Commit operation already in progress, skipping");
                return false;
            }
        }
        catch (ObjectDisposedException)
        {
            return false;
        }

        try
        {
            // Check again after acquiring lock
            if (_consumer == null || _disposed || (_reBalanceInProgress && !isShutdown))
                return false;

            if (_pendingOffsets.IsEmpty)
                return true; // Nothing to commit, consider it successful

            Interlocked.Increment(ref _totalCommitAttempts);

            // Safely copy pending offsets to avoid race conditions
            var pendingOffsetsCopy = _pendingOffsets.ToArray();
            if (pendingOffsetsCopy.Length == 0)
                return true;

            var offsetsToCommit = pendingOffsetsCopy
                .Select(kv => new TopicPartitionOffset(kv.Key, new Offset(kv.Value)))
                .ToList();

            try
            {
                // IMPORTANT: Only commit offsets we know were successfully processed
                var confirmedOffsets = offsetsToCommit
                    .Where(tpo => _processedMessages.ContainsKey(tpo))
                    .ToList();

                if (confirmedOffsets.Count == 0)
                {
                    _logger.LogWarning(
                        "No offsets to commit - messages may be processing but not yet confirmed successful");
                    return false;
                }

                // Commit first, only remove from tracking after successful commit
                _consumer.Commit(confirmedOffsets);

                var utcNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var timeSinceLastCommit = utcNow - _lastCommitTime;
                _lastCommitTime = utcNow;

                _logger.LogInformation("Committed {Count} offsets after {TimeSinceLastCommit}ms",
                    confirmedOffsets.Count, timeSinceLastCommit);

                // Only after successful commit:
                foreach (var offset in confirmedOffsets)
                {
                    // Remove from processed messages tracking
                    _processedMessages.TryRemove(offset, out _);

                    // Remove from pending offsets if this was the highest offset
                    _pendingOffsets.TryRemove(offset.TopicPartition, out _);
                }

                return true;
            }
            catch (KafkaException ex) when (ex.Error.Code == ErrorCode.InvalidGroupId
                                            || ex.Error.Code == ErrorCode.RebalanceInProgress
                                            || ex.Error.Reason.Contains("generation id is not valid"))
            {
                _logger.LogDebug(ex, "Offset commit skipped due to re-balance");
                Interlocked.Increment(ref _totalCommitFailures);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during offset commit: {Message}", ex.Message);
                Interlocked.Increment(ref _totalCommitFailures);
                return false;
            }
        }
        finally
        {
            try
            {
                _commitLock.Release();
            }
            catch (ObjectDisposedException)
            {
                // Nothing to do
            }
        }
    }

    private void LogPerformanceMetrics(object? state)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var elapsedSeconds = (now - _lastMetricsTimestamp) / 1000.0;

        if (elapsedSeconds <= 0)
            return;

        var received = Interlocked.Read(ref _totalMessagesReceived);
        var processed = Interlocked.Read(ref _totalMessagesProcessed);
        var skipped = Interlocked.Read(ref _totalSkippedMessages);
        var commitAttempts = Interlocked.Read(ref _totalCommitAttempts);
        var commitFailures = Interlocked.Read(ref _totalCommitFailures);
        var failed = Interlocked.Read(ref _totalFailedMessages);

        _logger.LogInformation(
            "Performance metrics: Received {ReceivedCount} messages ({ReceivedRate:F1}/sec), " +
            "Processed {ProcessedCount} messages ({ProcessedRate:F1}/sec), " +
            "Failed {FailedCount} messages, " +
            "Skipped {SkippedCount} messages, " +
            "Commit attempts: {CommitAttempts}, Commit failures: {CommitFailures}, " +
            "Preprocessing buffer: {PreprocessingCount}, " +
            "Processing buffer: {ProcessingCount}",
            received,
            received / elapsedSeconds,
            processed,
            processed / elapsedSeconds,
            failed,
            skipped,
            commitAttempts,
            commitFailures,
            _preprocessBlock.InputCount,
            _processingBlock.InputCount);

        // Reset counters
        Interlocked.Exchange(ref _totalMessagesReceived, 0);
        Interlocked.Exchange(ref _totalMessagesProcessed, 0);
        Interlocked.Exchange(ref _totalSkippedMessages, 0);
        Interlocked.Exchange(ref _totalFailedMessages, 0);
        Interlocked.Exchange(ref _totalCommitAttempts, 0);
        Interlocked.Exchange(ref _totalCommitFailures, 0);
        _lastMetricsTimestamp = now;
    }

    private async Task HandleError(Exception ex, string context, CancellationToken cancellationToken)
    {
        await _circuitBreakerLock.WaitAsync(cancellationToken);
        try
        {
            _consecutiveErrors++;

            _logger.LogError(ex, "{Context}: {Message} (ConsecutiveErrors={Count}/{Max})",
                context, ex.Message, _consecutiveErrors, _maxConsecutiveErrors);

            // Try to commit any pending offsets before any potential reset
            if (_consecutiveErrors >= _maxConsecutiveErrors / 2)
                try
                {
                    await CommitPendingOffsetsAsync(true).ConfigureAwait(false);
                    _logger.LogInformation("Emergency offset commit completed due to consecutive errors");
                }
                catch (Exception commitEx)
                {
                    _logger.LogError(commitEx, "Failed emergency offset commit during error handling");
                }

            if (_consecutiveErrors >= _maxConsecutiveErrors)
            {
                _logger.LogWarning(
                    "Circuit breaker triggered - Reinitializing consumer after {Count} consecutive errors",
                    _consecutiveErrors);

                // Reset the consumer and allow for full reinitializing
                try
                {
                    // Final attempt to commit before resetting consumer
                    await CommitPendingOffsetsAsync(true).ConfigureAwait(false);

                    _consumer?.Close();
                    _consumer?.Dispose();
                    _consumer = null;
                }
                catch (Exception closeEx)
                {
                    _logger.LogWarning(closeEx, "Error while closing consumer during circuit breaker reset");
                }

                // Small delay before reset to prevent thrashing
                await Task.Delay(5000, cancellationToken);

                // Reset error counter
                _consecutiveErrors = 0;
            }
            else
            {
                // Exponential backoff between 10ms and 1s
                var delayMs = Math.Min(1000, 10 * (1 << Math.Min(10, _consecutiveErrors)));
                await Task.Delay(delayMs, cancellationToken);
            }
        }
        finally
        {
            _circuitBreakerLock.Release();
        }
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _shuttingDown = true;
            _internalCts.Cancel();

            try
            {
                // Complete the pipeline
                _preprocessBlock.Complete();

                if (_consumer != null)
                    try
                    {
                        var pendingOffsetsCopy = _pendingOffsets.ToArray();
                        if (pendingOffsetsCopy.Length > 0)
                        {
                            var offsetsToCommit = pendingOffsetsCopy
                                .Select(kv => new TopicPartitionOffset(kv.Key, new Offset(kv.Value)))
                                .ToList();

                            _consumer.Commit(offsetsToCommit);
                            _logger.LogInformation("Committed {Count} offsets during shutdown", offsetsToCommit.Count);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error committing offsets during disposal");
                    }

                _locksDisposed = true;
                _commitLock.Dispose();
                _circuitBreakerLock.Dispose();
                _internalCts.Dispose();
                _commitTimer.Dispose();
                _metricsTimer.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during KafkaSubscriber disposal");
            }
        }

        _disposed = true;
    }

    ~KafkaV2Subscriber()
    {
        Dispose(false);
    }
}