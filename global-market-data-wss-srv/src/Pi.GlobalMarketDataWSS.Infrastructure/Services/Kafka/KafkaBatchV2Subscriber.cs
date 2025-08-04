using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketDataWSS.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataWSS.Infrastructure.Extensions;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Kafka;
using Polly;
using Polly.Retry;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Services.Kafka;

public sealed class KafkaBatchV2Subscriber<TKey, TValue> : IKafkaBatchV2Subscriber, IDisposable where TKey : notnull
{
    // Configuration fields
    private readonly int _batchSize;
    private readonly IConfiguration _configuration;
    private readonly CancellationTokenSource _internalCts = new();
    private readonly ILogger<KafkaBatchV2Subscriber<TKey, TValue>> _logger;
    private readonly int _maxRetryAttempts;
    private readonly IKafkaMessageHandler<Message<TKey, TValue>> _messageHandler;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly string _topic;

    // State tracking
    private IConsumer<TKey, TValue>? _consumer;
    private bool _disposed;
    private bool _isSubscribed;

    public KafkaBatchV2Subscriber(
        IConfiguration configuration,
        string topic,
        IKafkaMessageHandler<Message<TKey, TValue>> messageHandler,
        ILogger<KafkaBatchV2Subscriber<TKey, TValue>> logger
    )
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _topic = topic ?? throw new ArgumentNullException(nameof(topic));
        _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Load configuration with optimized defaults
        _batchSize = 100;
        _maxRetryAttempts = _configuration.GetValue("KAFKA:CONSUMER_MAX_RETRY_ATTEMPTS", 5);
        var retryDelayMs = _configuration.GetValue("KAFKA:CONSUMER_RETRY_DELAY_MS", 1000);


        // Create retry policy with exponential backoff
        _retryPolicy = Policy
            .Handle<KafkaException>()
            .Or<ConsumeException>()
            .WaitAndRetryAsync(
                _maxRetryAttempts,
                retryAttempt => TimeSpan.FromMilliseconds(retryDelayMs * Math.Pow(2, retryAttempt - 1)),
                (ex, timeSpan, retryCount, _) =>
                {
                    if (retryCount == _maxRetryAttempts)
                        _logger.LogError(ex, "Kafka connection failed after {MaxRetries} attempts.", _maxRetryAttempts);
                    else
                        _logger.LogWarning(ex, "Retrying Kafka connection in {DelayMs}ms...",
                            timeSpan.TotalMilliseconds);
                });
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _internalCts.Cancel();
        _internalCts.Dispose();

        try
        {
            // Safely close and dispose consumer
            if (_consumer != null)
            {
                _consumer.Close();
                _consumer.Dispose();
                _consumer = null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during consumer disposal");
        }

        _disposed = true;
    }

    public async Task SubscribeAsync(CancellationToken cancellationToken)
    {
        if (!_disposed)
        {
            if (_isSubscribed)
            {
                _logger.LogWarning("Consumer is already subscribed to topic: {Topic}", _topic);
                return;
            }

            var config = BuildConsumerConfig();

            try
            {
                await ExecuteAsync(config, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Propagate cancellation exceptions
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to subscribe to Kafka topic {Topic} after {MaxRetries} attempts", _topic,
                    _maxRetryAttempts);
                throw new InvalidOperationException($"Failed to subscribe to Kafka topic {_topic}", ex);
            }
        }
        else
        {
            throw new ObjectDisposedException(nameof(KafkaBatchV2Subscriber<TKey, TValue>));
        }
    }

    public async Task UnsubscribeAsync()
    {
        if (!_disposed)
        {
            if (!_isSubscribed)
            {
                _logger.LogDebug("No active subscription to unsubscribe from.");
                return;
            }

            try
            {
                await _internalCts.CancelAsync();

                if (_consumer != null)
                {
                    _consumer.Unsubscribe();
                    _consumer.Close();
                    _consumer.Dispose();
                    _consumer = null;
                }

                _isSubscribed = false;
                _logger.LogInformation("Successfully unsubscribed from Kafka topic: {Topic}", _topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while unsubscribing from Kafka topic: {Topic}", _topic);
                throw new InvalidOperationException(ex.Message, ex);
            }
        }
        else
        {
            throw new ObjectDisposedException(nameof(KafkaBatchV2Subscriber<TKey, TValue>));
        }
    }

    private async Task ExecuteAsync(ConsumerConfig config, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            _consumer = new ConsumerBuilder<TKey, TValue>(config)
                .SetErrorHandler((_, error) =>
                {
                    _logger.LogError("Kafka error: {Reason} (Code: {Code})", error.Reason, error.Code);
                    if (error.IsFatal)
                    {
                        _logger.LogWarning("Fatal Kafka error detected. Restarting consumer...");
                        Task.Run(RestartConsumerAsync, cancellationToken);
                    }
                })
                .SetPartitionsAssignedHandler((_, partitions) =>
                {
                    var partitionValues = string.Join(", ", partitions.Select(p => p.Partition.Value));
                    _logger.LogDebug("Assigned partitions: {Partitions}", partitionValues);
                })
                .SetPartitionsRevokedHandler((_, partitions) =>
                {
                    _logger.LogDebug("Revoked partitions: {Partitions}",
                        string.Join(", ", partitions.Select(p => p.Partition.Value)));
                })
                .Build();

            _consumer.Subscribe(_topic);
            _isSubscribed = true;
            _logger.LogInformation("Successfully subscribed to Kafka topic: {Topic}", _topic);

            using var linkedToken =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _internalCts.Token);
            await Task.Run(() => HandleConsumeAsync(linkedToken.Token), linkedToken.Token);
        });
    }

    private ConsumerConfig BuildConsumerConfig()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration[ConfigurationKeys.KafkaBootstrapServers],
            GroupId = _configuration[ConfigurationKeys.KafkaConsumerGroupId],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,

            // Optimized settings for higher throughput
            FetchMinBytes = _configuration.GetValue("KAFKA:CONSUMER_FETCH_MIN_BYTES", 64 * 1024), // 64KB
            FetchWaitMaxMs = _configuration.GetValue("KAFKA:CONSUMER_FETCH_WAIT_MAX_MS", 10),
            SessionTimeoutMs = _configuration.GetValue("KAFKA:CONSUMER_SESSION_TIMEOUT_MS", 45000),
            MaxPollIntervalMs = _configuration.GetValue("KAFKA:CONSUMER_MAX_POLL_INTERVAL_MS", 600000),
            MaxPartitionFetchBytes =
                _configuration.GetValue("KAFKA:CONSUMER_MAX_PARTITION_FETCH_BYTES", 8 * 1024 * 1024), // 8MB
            FetchMaxBytes = _configuration.GetValue("KAFKA:CONSUMER_FETCH_MAX_BYTES", 100 * 1024 * 1024), // 100MB
            QueuedMinMessages = _configuration.GetValue("KAFKA:CONSUMER_QUEUED_MIN_MESSAGES", 100000),
            QueuedMaxMessagesKbytes =
                _configuration.GetValue("KAFKA:CONSUMER_QUEUED_MAX_MESSAGES_KBYTES", 2 * 1024 * 1024), // 2GB

            // Additional settings for better error detection
            AllowAutoCreateTopics = false,
            EnablePartitionEof = true,

            // Socket settings optimized for performance
            SocketTimeoutMs = _configuration.GetValue("KAFKA:CONSUMER_SOCKET_TIMEOUT_MS", 120000),
            SocketKeepaliveEnable = true,

            // Performance enhancements (disable statistics)
            StatisticsIntervalMs = 0,
            SocketReceiveBufferBytes = 2 * 1024 * 1024, // 2MB

            // Optimize for consumer group stability
            HeartbeatIntervalMs = _configuration.GetValue("KAFKA:CONSUMER_HEARTBEAT_INTERVAL_MS", 3000),

            // Increase message size limit if needed
            MessageMaxBytes = _configuration.GetValue("KAFKA:CONSUMER_MESSAGE_MAX_BYTES", 50 * 1024 * 1024) // 50MB
        };

        try
        {
            var securityProtocolString = _configuration[ConfigurationKeys.KafkaSecurityProtocol] ?? "SASL_SSL";
            config.SecurityProtocol = Enum.TryParse<SecurityProtocol>(securityProtocolString.Replace("_", string.Empty),
                true,
                out var securityProtocol)
                ? securityProtocol
                : SecurityProtocol.SaslSsl;

            var saslMechanismString = _configuration[ConfigurationKeys.KafkaSaslMechanism] ?? "PLAIN";
            config.SaslMechanism = Enum.TryParse<SaslMechanism>(saslMechanismString.Replace("_", string.Empty), true,
                out var saslMechanism)
                ? saslMechanism
                : SaslMechanism.Plain;

            config.SaslUsername = _configuration[ConfigurationKeys.KafkaSaslUsername];
            config.SaslPassword = _configuration[ConfigurationKeys.KafkaSaslPassword];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error configuring Kafka security settings: {Message}", ex.Message);
            throw new InvalidOperationException("Failed to configure Kafka security settings", ex);
        }

        return config;
    }

    private async Task RestartConsumerAsync()
    {
        try
        {
            _logger.LogWarning("Restarting Kafka consumer for topic: {Topic}", _topic);

            if (_consumer != null)
            {
                try
                {
                    _consumer.Close();
                    _consumer.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error while closing consumer during restart");
                }

                _consumer = null;
            }

            _isSubscribed = false;

            // Pause before restart
            await Task.Delay(2000);
            await SubscribeAsync(_internalCts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during consumer restart");
        }
    }

    private async Task HandleConsumeAsync(CancellationToken cancellationToken)
    {
        // List to track active processing tasks
        var processingTasks = new List<Task>();

        // Warm up handling components
        await Task.Yield();

        while (!cancellationToken.IsCancellationRequested && _consumer != null)
            try
            {
                // Remove tasks from the list
                processingTasks.RemoveAll(task => task.IsCompleted || task.IsFaulted || task.IsCanceled);

                // Wait if we've reached the maximum number of concurrent tasks
                if (processingTasks.Count >= 500)
                {
                    // Wait for any task to complete before continuing
                    await Task.WhenAny(processingTasks);
                    continue;
                }

                // Consume batch of messages
                var messageBatch = _consumer.ConsumeBatchV2(
                    TimeSpan.FromMilliseconds(100),
                    _batchSize,
                    200,
                    cancellationToken).ToList();

                if (messageBatch.Count > 0)
                {
                    // Create and start a new task for processing this batch
                    var processingTask = Task.Run(() => ProcessMessageBatchAsync(messageBatch), cancellationToken);
                    processingTasks.Add(processingTask);

                    // Log batch processing
                    _logger.LogDebug("Started processing task for batch of {Count} messages. Active tasks: {TaskCount}",
                        messageBatch.Count, processingTasks.Count);
                }
                else
                {
                    // Only wait a short time if no messages to minimize lag
                    await Task.Delay(1, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (KafkaException ex)
            {
                _logger.LogError(ex, "Kafka error detected. Restarting consumer...");
                await RestartConsumerAsync();
                await Task.Delay(1000, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in main consumption loop: {Message}", ex.Message);
                await Task.Delay(1000, cancellationToken);
            }

        _logger.LogInformation("Exiting consumption loop, waiting for {Count} tasks to complete",
            processingTasks.Count);

        // Wait for all remaining tasks to complete before exiting
        await Task.WhenAll(processingTasks);
    }

    private async Task ProcessMessageBatchAsync(IReadOnlyCollection<ConsumeResult<TKey, TValue>> messageBatch)
    {
        if (messageBatch.Count == 0)
            return;

        await Task.WhenAll(messageBatch.Select(message => 
            _messageHandler.HandleAsync(message.Message)));
    }
}