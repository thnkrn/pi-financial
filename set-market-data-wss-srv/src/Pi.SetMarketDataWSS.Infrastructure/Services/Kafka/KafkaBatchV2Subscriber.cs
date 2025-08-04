using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataWSS.Domain.ConstantConfigurations;
using Pi.SetMarketDataWSS.Infrastructure.Extensions;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Kafka;
using Polly;
using Polly.Retry;

namespace Pi.SetMarketDataWSS.Infrastructure.Services.Kafka;

public sealed class KafkaBatchV2Subscriber<TKey, TValue> : IKafkaBatchV2Subscriber, IDisposable where TKey : notnull
{
    private const string SequenceNumberKey = "sequenceNumber";

    // Batch size is now tunable
    private readonly int _batchSize;
    private readonly int _commitIntervalMs;
    private readonly IConfiguration _configuration;

    // Object pool for dictionaries to reduce GC pressure
    private readonly ConcurrentBag<Dictionary<TKey, (ConsumeResult<TKey, TValue> Message, long SequenceNumber)>>
        _dictionaryPool;

    private readonly CancellationTokenSource _internalCts = new();
    private readonly ILogger<KafkaBatchV2Subscriber<TKey, TValue>> _logger;
    private readonly int _maxBatchProcessingTimeMs;
    private readonly int _maxConcurrentPartitions;
    private readonly int _maxRetryAttempts;
    private readonly IKafkaMessageHandler<Message<TKey, TValue>> _messageHandler;
    private readonly SemaphoreSlim _partitionSemaphore;
    private readonly ConcurrentDictionary<TopicPartition, TopicPartitionOffset> _pendingOffsets = new();
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly string _topic;

    private IConsumer<TKey, TValue>? _consumer;
    private bool _disposed;
    private bool _isSubscribed;
    private DateTime _lastCommitTime = DateTime.UtcNow;

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
        _batchSize = _configuration.GetValue("KAFKA:CONSUMER_BATCH_SIZE", 500); // Increased batch size
        _maxBatchProcessingTimeMs =
            _configuration.GetValue("KAFKA:CONSUMER_MAX_BATCH_PROCESSING_TIME_MS", 5000); // Increased processing time
        _maxRetryAttempts = _configuration.GetValue("KAFKA:CONSUMER_MAX_RETRY_ATTEMPTS", 5);
        _commitIntervalMs = _configuration.GetValue("KAFKA:CONSUMER_COMMIT_INTERVAL_MS", 5000); // Less frequent commits
        var retryDelayMs = _configuration.GetValue("KAFKA:CONSUMER_RETRY_DELAY_MS", 1000);

        // Increase concurrent partitions for better parallelism
        _maxConcurrentPartitions = _configuration.GetValue("KAFKA:MAX_CONCURRENT_PARTITIONS",
            Math.Max(Environment.ProcessorCount, 4)); // At least use all cores
        _partitionSemaphore = new SemaphoreSlim(_maxConcurrentPartitions, _maxConcurrentPartitions);

        // Initialize dictionary pool
        _dictionaryPool = [];

        // Create retry policy
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

        if (_batchSize > 1)
            _batchSize = 1;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _internalCts.Cancel();
        _internalCts.Dispose();
        _consumer?.Close();
        _consumer?.Dispose();
        _partitionSemaphore.Dispose();
        _disposed = true;
    }

    public async Task SubscribeAsync(CancellationToken cancellationToken)
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
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to subscribe to Kafka topic {Topic} after {MaxRetries} attempts", _topic,
                _maxRetryAttempts);
            throw new InvalidOperationException($"Failed to subscribe to Kafka topic {_topic}", ex);
        }
    }

    public async Task UnsubscribeAsync()
    {
        if (!_isSubscribed)
        {
            _logger.LogDebug("No active subscription to unsubscribe from.");
            return;
        }

        try
        {
            await _internalCts.CancelAsync();

            // Ensure all pending offsets are committed before unsubscribing
            if (!_pendingOffsets.IsEmpty && _consumer != null)
                try
                {
                    _consumer.Commit(_pendingOffsets.Values);
                    _logger.LogInformation("Final commit of {Count} offsets before unsubscribing",
                        _pendingOffsets.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to commit final offsets before unsubscribing");
                }

            _consumer?.Unsubscribe();
            _consumer?.Dispose();
            _consumer = null;
            _isSubscribed = false;
            _logger.LogInformation("Successfully unsubscribed from Kafka topic: {Topic}", _topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while unsubscribing from Kafka topic: {Topic}", _topic);
            throw new InvalidOperationException(ex.Message, ex);
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
                    _logger.LogInformation("Assigned partitions: {Partitions}", partitionValues);
                })
                .SetPartitionsRevokedHandler((_, partitions) =>
                {
                    _logger.LogInformation("Revoked partitions: {Partitions}",
                        string.Join(", ", partitions.Select(p => p.Partition.Value)));

                    // Force commit before partitions are revoked
                    if (!_pendingOffsets.IsEmpty && _consumer != null)
                        try
                        {
                            _consumer.Commit(_pendingOffsets.Values);
                            _logger.LogInformation("Committed offsets before partition revocation");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to commit offsets before partition revocation");
                        }
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
            EnableAutoCommit = false,

            // Optimized settings for higher throughput
            FetchMinBytes = _configuration.GetValue("KAFKA:CONSUMER_FETCH_MIN_BYTES", 64 * 1024), // 64KB (increased)
            FetchWaitMaxMs = _configuration.GetValue("KAFKA:CONSUMER_FETCH_WAIT_MAX_MS", 10),
            SessionTimeoutMs = _configuration.GetValue("KAFKA:CONSUMER_SESSION_TIMEOUT_MS", 45000), // Increased
            MaxPollIntervalMs = _configuration.GetValue("KAFKA:CONSUMER_MAX_POLL_INTERVAL_MS", 600000), // Increased
            MaxPartitionFetchBytes =
                _configuration.GetValue("KAFKA:CONSUMER_MAX_PARTITION_FETCH_BYTES", 8 * 1024 * 1024), // 8MB (increased)
            FetchMaxBytes =
                _configuration.GetValue("KAFKA:CONSUMER_FETCH_MAX_BYTES", 100 * 1024 * 1024), // 100MB (increased)
            QueuedMinMessages = _configuration.GetValue("KAFKA:CONSUMER_QUEUED_MIN_MESSAGES", 100000), // Increased
            QueuedMaxMessagesKbytes =
                _configuration.GetValue("KAFKA:CONSUMER_QUEUED_MAX_MESSAGES_KBYTES",
                    2 * 1024 * 1024), // 2GB (increased)

            // Additional settings for better error detection
            AllowAutoCreateTopics = false,
            EnablePartitionEof = true,

            // Socket settings optimized for performance
            SocketTimeoutMs = _configuration.GetValue("KAFKA:CONSUMER_SOCKET_TIMEOUT_MS", 120000), // Increased timeout
            SocketKeepaliveEnable = true,

            // Performance enhancements (disable statistics)
            StatisticsIntervalMs = 0,
            SocketReceiveBufferBytes = 2 * 1024 * 1024, // 2MB (increased)

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
        _consumer?.Close();
        _consumer?.Dispose();
        _consumer = null;
        _isSubscribed = false;

        // Pause before restart
        await Task.Delay(2000);
        await SubscribeAsync(_internalCts.Token);
    }

    private async Task HandleConsumeAsync(CancellationToken cancellationToken)
    {
        // Warm up handling components
        await Task.Yield(); // Allow the thread to be scheduled optimally

        while (!cancellationToken.IsCancellationRequested)
        {
            var consumeResult = await GetResult(cancellationToken);

            if (consumeResult.IsBreak)
                break;
        }
    }

    private async Task<(bool IsBreak, int MessageCount)> GetResult(CancellationToken cancellationToken)
    {
        try
        {
            // Use a more efficient approach to consume batches
            var messageBatch = _consumer?.ConsumeBatchV2(TimeSpan.FromMilliseconds(_maxBatchProcessingTimeMs),
                _batchSize, 100, cancellationToken).ToList(); // Increased partition fill time for better batching

            if (messageBatch?.Count > 0)
            {
                var batchSize = messageBatch.Count;
                await ProcessMessageBatchAsync(messageBatch);

                // Commit less frequently for better throughput
                var shouldCommit =
                    (DateTime.UtcNow - _lastCommitTime).TotalMilliseconds >= _commitIntervalMs ||
                    _pendingOffsets.Count >= _batchSize * 10; // Increased threshold

                if (shouldCommit && !_pendingOffsets.IsEmpty)
                {
                    CommitOffsets();
                    _pendingOffsets.Clear();
                }

                return (false, batchSize);
            }

            if (messageBatch is not { Count: > 0 })
                await Task.Delay(1, cancellationToken); // Only wait a short time if no messages to minimize lag
        }
        catch (KafkaException ex)
        {
            _logger.LogError(ex, "Kafka error detected. Restarting consumer...");
            await RestartConsumerAsync();
        }
        catch (OperationCanceledException)
        {
            if (!_pendingOffsets.IsEmpty)
                CommitOffsets();
            return (true, 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while consuming messages.");
            await Task.Delay(1000, cancellationToken);
        }

        return (false, 0);
    }

    private async Task ProcessMessageBatchAsync(IReadOnlyCollection<ConsumeResult<TKey, TValue>> messageBatch)
    {
        if (messageBatch.Count == 0) return;

        // Fast path: Most common case is having just one partition in the batch
        if (messageBatch.All(m => m.TopicPartition.Equals(messageBatch.First().TopicPartition)))
        {
            await ProcessPartitionMessagesAsync(
                messageBatch,
                messageBatch.First().TopicPartition);
        }
        else
        {
            // Use more efficient processing with DataFlow
            var partitionGroups = messageBatch
                .GroupBy(m => m.TopicPartition)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Use TPL Dataflow for more efficient parallelism with better backpressure handling
            var actionBlock = new ActionBlock<KeyValuePair<TopicPartition, List<ConsumeResult<TKey, TValue>>>>(
                async partitionGroup =>
                {
                    // Using semaphore to limit concurrent partition processing
                    await _partitionSemaphore.WaitAsync();
                    try
                    {
                        await ProcessPartitionMessagesAsync(
                            partitionGroup.Value,
                            partitionGroup.Key);
                    }
                    finally
                    {
                        _partitionSemaphore.Release();
                    }
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = _maxConcurrentPartitions,
                    BoundedCapacity = partitionGroups.Count,
                    SingleProducerConstrained = true // Optimization for single producer scenario
                });

            // Process each partition with controlled parallelism
            foreach (var partitionGroup in partitionGroups)
                await actionBlock.SendAsync(partitionGroup);

            // Wait for all partitions to complete processing
            actionBlock.Complete();
            await actionBlock.Completion;
        }
    }

    private async Task ProcessPartitionMessagesAsync(
        IEnumerable<ConsumeResult<TKey, TValue>> partitionMessages, TopicPartition partition)
    {
        try
        {
            var maxOffset = 0L;
            var messages = partitionMessages.ToArray();

            // Get or create dictionary from pool to reduce allocations
            if (!_dictionaryPool.TryTake(out var latestMessageBySymbol))
                latestMessageBySymbol =
                    new Dictionary<TKey, (ConsumeResult<TKey, TValue> Message, long SequenceNumber)>(1024);
            else
                latestMessageBySymbol.Clear();

            try
            {
                foreach (var result in messages)
                {
                    // Track max offset
                    var resultOffset = result.Offset;
                    if (resultOffset > maxOffset)
                        maxOffset = resultOffset;

                    var key = result.Message.Key;
                    var sequenceNumber = GetSequenceNumberFast(result.Message.Headers, 1);

                    if (!latestMessageBySymbol.TryGetValue(key, out var existing) ||
                        sequenceNumber > existing.SequenceNumber)
                        latestMessageBySymbol[key] = (result, sequenceNumber);
                }

                _pendingOffsets[partition] = new TopicPartitionOffset(partition, maxOffset + 1);

                // Process only the latest message for each key
                _ = await ProcessLatestMessagesAsync(latestMessageBySymbol.Values);
            }
            finally
            {
                // Return dictionary to pool
                _dictionaryPool.Add(latestMessageBySymbol);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process latest messages in partition {Partition}", partition);
        }
    }

    private async Task<int> ProcessLatestMessagesAsync(
        IEnumerable<(ConsumeResult<TKey, TValue> Message, long SequenceNumber)> messages)
    {
        var processedCount = 0;
        var msgList = messages.ToList();

        // Optimize: Adjust concurrency based on message count and available processors
        var concurrency = DetermineBestConcurrency(msgList.Count);

        // For small batches or when concurrency is 1, process sequentially to avoid parallelization overhead
        if (concurrency <= 1)
        {
            foreach (var item in msgList)
            {
                await _messageHandler.HandleAsync(item.Message.Message);
                processedCount++;
            }

            return processedCount;
        }

        // For larger batches, use optimized parallelization with a shared counter
        var localProcessedCount = 0;
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = concurrency
        };

        await Parallel.ForEachAsync(
            msgList,
            parallelOptions,
            async (item, _) =>
            {
                await _messageHandler.HandleAsync(item.Message.Message);
                Interlocked.Increment(ref localProcessedCount);
            });

        processedCount = localProcessedCount;
        return processedCount;
    }

    // Optimized method to determine best concurrency
    private int DetermineBestConcurrency(int messageCount)
    {
        // For very small batches, sequential processing may be more efficient
        if (messageCount <= 10)
            return 1;

        // For medium batches, use a portion of available processors
        return messageCount <= 100
            ? Math.Max(1, _maxConcurrentPartitions / 2)
            : _maxConcurrentPartitions; // For large batches, use full capacity
    }

    // Optimized sequence number extraction with spans
    private static long GetSequenceNumberFast(Headers headers, int sequenceNumberIndex)
    {
        // Common case: short-circuit check at index where sequence typically sits
        if (headers.Count > sequenceNumberIndex && headers[sequenceNumberIndex].Key == SequenceNumberKey)
        {
            ReadOnlySpan<byte> valueBytes = headers[sequenceNumberIndex].GetValueBytes();
            if (valueBytes.Length == 8)
                return BitConverter.ToInt64(valueBytes);
        }

        // Fall back to searching if not found at the expected indexes
        foreach (var header in headers)
            if (header.Key == SequenceNumberKey)
            {
                ReadOnlySpan<byte> bytes = header.GetValueBytes();
                if (bytes.Length == 8)
                    return BitConverter.ToInt64(bytes);
            }

        return 0;
    }

    private void CommitOffsets()
    {
        try
        {
            if (_consumer != null && !_pendingOffsets.IsEmpty)
            {
                _consumer.Commit(_pendingOffsets.Values);
                _lastCommitTime = DateTime.UtcNow;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit offsets");
        }
    }
}