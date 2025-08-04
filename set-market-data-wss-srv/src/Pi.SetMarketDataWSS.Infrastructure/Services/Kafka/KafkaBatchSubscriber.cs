using System.Collections.Concurrent;
using System.Diagnostics;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataWSS.Domain.ConstantConfigurations;
using Pi.SetMarketDataWSS.Infrastructure.Extensions;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Kafka;
using Polly;
using Polly.Retry;

namespace Pi.SetMarketDataWSS.Infrastructure.Services.Kafka;

public sealed class KafkaBatchSubscriber<TKey, TValue> : IKafkaBatchSubscriber, IDisposable where TKey : notnull
{
    private const string SequenceNumberKey = "sequenceNumber";
    private readonly int _commitIntervalMs;
    private readonly IConfiguration _configuration;
    private readonly CancellationTokenSource _internalCts = new();
    private readonly ILogger<KafkaBatchSubscriber<TKey, TValue>> _logger;
    private readonly int _maxRetryAttempts;
    private readonly IKafkaMessageHandler<TValue> _messageHandler;
    private readonly ConcurrentDictionary<int, long> _messagesByPartition = new();
    private readonly ConcurrentDictionary<TopicPartition, TopicPartitionOffset> _pendingOffsets = new();
    private readonly ConcurrentDictionary<int, long> _processingTimeByPartition = new();
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly string _topic;

    // Performance monitoring
    private readonly Stopwatch _uptimeStopwatch = Stopwatch.StartNew();
    private int _batchSize;

    private IConsumer<TKey, TValue>? _consumer;
    private bool _disposed;
    private bool _isSubscribed;
    private DateTime _lastCommitTime = DateTime.UtcNow;
    private long _lastMinuteMessages;
    private int _maxBatchProcessingTimeMs;
    private long _totalBatchesProcessed;
    private long _totalCommits;
    private long _totalMessagesConsumed;

    // Added for optimization metrics
    private long _totalMessagesSkipped;
    private long _totalProcessedMessages;

    public KafkaBatchSubscriber(
        IConfiguration configuration,
        string topic,
        IKafkaMessageHandler<TValue> messageHandler,
        ILogger<KafkaBatchSubscriber<TKey, TValue>> logger
    )
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                          ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _topic = topic ?? throw new ArgumentNullException(nameof(topic));
        _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Load configuration with defaults - increased for better throughput
        _batchSize = _configuration.GetValue("KAFKA:CONSUMER_BATCH_SIZE", 500); // Increased from 100
        _maxBatchProcessingTimeMs =
            _configuration.GetValue("KAFKA:CONSUMER_MAX_BATCH_PROCESSING_TIME_MS", 2000); // Increased from 1000
        _maxRetryAttempts = _configuration.GetValue("KAFKA:CONSUMER_MAX_RETRY_ATTEMPTS", 5);
        _commitIntervalMs = _configuration.GetValue("KAFKA:CONSUMER_COMMIT_INTERVAL_MS", 2000); // Reduced from 5000
        var retryDelayMs = _configuration.GetValue("KAFKA:CONSUMER_RETRY_DELAY_MS", 1000);

        // Validate configuration
        ValidateConfiguration();

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

        // Start performance monitoring
        if (!string.IsNullOrEmpty(environment) &&
            !environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
            StartPerformanceMetricsLogger();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _internalCts.Cancel();
        _internalCts.Dispose();
        _consumer?.Close();
        _consumer?.Dispose();
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

                        // Initialize performance tracking for each partition
                        foreach (var partitionValue in partitions.Select(p => p.Partition.Value))
                        {
                            _messagesByPartition.AddOrUpdate(partitionValue, 0, (_, existing) => existing);
                            _processingTimeByPartition.AddOrUpdate(partitionValue, 0, (_, existing) => existing);
                        }
                    })
                    .SetPartitionsRevokedHandler((_, partitions) =>
                    {
                        _logger.LogInformation("Revoked partitions: {Partitions}",
                            string.Join(", ", partitions.Select(p => p.Partition.Value)));

                        // Force commit before partitions are revoked
                        if (!_pendingOffsets.IsEmpty)
                            try
                            {
                                _consumer?.Commit(_pendingOffsets.Values);
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

    private ConsumerConfig BuildConsumerConfig()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration[ConfigurationKeys.KafkaBootstrapServers],
            GroupId = _configuration[ConfigurationKeys.KafkaConsumerGroupId],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,

            // Near real-time optimization settings with proper validation
            FetchMinBytes = _configuration.GetValue("KAFKA:CONSUMER_FETCH_MIN_BYTES", 1), // Increased from 1024
            FetchWaitMaxMs = _configuration.GetValue("KAFKA:CONSUMER_FETCH_WAIT_MAX_MS", 10), // Reduced from 250
            SessionTimeoutMs =
                _configuration.GetValue("KAFKA:CONSUMER_SESSION_TIMEOUT_MS", 30000), // Increased from 10000
            MaxPollIntervalMs =
                _configuration.GetValue("KAFKA:CONSUMER_MAX_POLL_INTERVAL_MS", 300000), // Increased from 30000
            MaxPartitionFetchBytes =
                _configuration.GetValue("KAFKA:CONSUMER_MAX_PARTITION_FETCH_BYTES", 4194304), // Increased from 1MB
            FetchMaxBytes = _configuration.GetValue("KAFKA:CONSUMER_FETCH_MAX_BYTES", 52428800),
            QueuedMinMessages =
                _configuration.GetValue("KAFKA:CONSUMER_QUEUED_MIN_MESSAGES", 10000), // Increased from 1000
            QueuedMaxMessagesKbytes = _configuration.GetValue("KAFKA:CONSUMER_QUEUED_MAX_MESSAGES_KBYTES", 1048576),

            // Additional settings for better error detection
            AllowAutoCreateTopics = false,
            EnablePartitionEof = true,

            // Socket settings
            SocketTimeoutMs = _configuration.GetValue("KAFKA:CONSUMER_SOCKET_TIMEOUT_MS", 60000),
            SocketKeepaliveEnable = true,

            // Performance enhancements
            StatisticsIntervalMs = 60000, // Enable statistics every minute
            SocketReceiveBufferBytes = 1048576 // 1MB socket buffer
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
        var lastLogTime = DateTime.UtcNow;
        var messagesLastMinute = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (await GetResult(cancellationToken))
                break;

            // Log throughput every minute
            if ((DateTime.UtcNow - lastLogTime).TotalMinutes >= 1)
            {
                Interlocked.Exchange(ref _lastMinuteMessages, messagesLastMinute);
                _logger.LogInformation("Throughput: {MessagesPerMinute} messages/minute", messagesLastMinute);
                messagesLastMinute = 0;
                lastLogTime = DateTime.UtcNow;
            }
        }
    }

    private async Task<bool> GetResult(CancellationToken cancellationToken)
    {
        var batchStopwatch = Stopwatch.StartNew();
        try
        {
            var messageBatch = _consumer?.ConsumeBatch(TimeSpan.FromMilliseconds(_maxBatchProcessingTimeMs), _batchSize,
                cancellationToken).ToList();

            if (messageBatch is { Count: > 0 })
            {
                Interlocked.Increment(ref _totalBatchesProcessed);
                var batchSize = messageBatch.Count;
                Interlocked.Add(ref _totalMessagesConsumed, batchSize);

                var processingStopwatch = Stopwatch.StartNew();
                await ProcessMessageBatchAsync(messageBatch);
                processingStopwatch.Stop();

                var processingTime = processingStopwatch.ElapsedMilliseconds;

                // Track processing time and message count per partition
                foreach (var partitionId in messageBatch.Select(message => message.Partition.Value))
                {
                    _messagesByPartition.AddOrUpdate(
                        partitionId,
                        1,
                        (_, count) => count + 1);

                    _processingTimeByPartition.AddOrUpdate(
                        partitionId,
                        processingTime / batchSize, // distribute time equally among messages
                        (_, time) => time + processingTime / batchSize);
                }

                // Always commit if batch size is large enough or enough time has passed
                var shouldCommit =
                    (DateTime.UtcNow - _lastCommitTime).TotalMilliseconds >= _commitIntervalMs ||
                    _pendingOffsets.Count >= _batchSize * 4;

                if (shouldCommit && !_pendingOffsets.IsEmpty)
                {
                    CommitOffsets();
                    _pendingOffsets.Clear();
                }

                // Log batch processing metrics
                batchStopwatch.Stop();
                if (batchStopwatch.ElapsedMilliseconds > 1000) // Only log slow batches
                    _logger.LogInformation(
                        "Batch processing: {Count} messages in {TotalMs}ms, processing: {ProcessingMs}ms, " +
                        "rate: {MessagesPerSecond}/sec",
                        batchSize,
                        batchStopwatch.ElapsedMilliseconds,
                        processingTime,
                        batchSize * 1000 / batchStopwatch.ElapsedMilliseconds);
            }

            if (messageBatch is not { Count: > 0 })
                // Only wait a short time if no messages to minimize lag
                await Task.Delay(1, cancellationToken);
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
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while consuming messages.");
            await Task.Delay(1000, cancellationToken);
        }

        return false;
    }

    private async Task ProcessMessageBatchAsync(IReadOnlyCollection<ConsumeResult<TKey, TValue>> messageBatch)
    {
        if (messageBatch.Count == 0) return;

        // Avoid string interpolation in hot paths
        _logger.LogInformation("Processing batch of {Count} messages", messageBatch.Count);

        // Use regular dictionary instead of concurrent for local usage within this method
        var partitionMetrics = new Dictionary<int, (int TotalMessages, int ProcessedCount, long ProcessingTime)>();

        // Fast path: Most common case is having just one partition in the batch
        if (messageBatch.All(m => m.TopicPartition.Equals(messageBatch.First().TopicPartition)))
        {
            await ProcessPartitionMessagesAsync(
                messageBatch,
                messageBatch.First().TopicPartition,
                partitionMetrics);
        }
        else
        {
            // Process by partition groups to maintain ordering within each partition
            // Pre-allocate to reduce GC pressure
            var partitionGroups = new Dictionary<TopicPartition, List<ConsumeResult<TKey, TValue>>>();

            // Group by TopicPartition manually instead of using LINQ GroupBy
            foreach (var message in messageBatch)
            {
                if (!partitionGroups.TryGetValue(message.TopicPartition, out var list))
                {
                    list = new List<ConsumeResult<TKey, TValue>>(messageBatch.Count / 4); // Estimate size
                    partitionGroups[message.TopicPartition] = list;
                }

                list.Add(message);
            }

            // Create all tasks up front
            var partitionTasks = new Task[partitionGroups.Count];
            var taskIndex = 0;

            foreach (var (partition, messages) in partitionGroups)
                // Use ValueTask for less allocation
                partitionTasks[taskIndex++] = ProcessPartitionMessagesAsync(
                    messages,
                    partition,
                    partitionMetrics);

            // Wait for all partitions to complete
            await Task.WhenAll(partitionTasks);
        }

        // Log metrics after all processing is done
        foreach (var (partitionId, value) in partitionMetrics)
        {
            var (totalMessages, processedCount, processingTime) = value;
            var skippedCount = totalMessages - processedCount;

            // Only log if we have messages or took significant time
            if (totalMessages > 0 || processingTime > 100)
                _logger.LogInformation(
                    "Partition {Partition} optimization: processed {Processed}/{Total} messages (skipped {Skipped}) in {ElapsedMs}ms",
                    partitionId,
                    processedCount,
                    totalMessages,
                    skippedCount,
                    processingTime);

            // Update global metrics only once
            Interlocked.Add(ref _totalProcessedMessages, processedCount);
            Interlocked.Add(ref _totalMessagesSkipped, skippedCount);

            // Update partition tracking
            _messagesByPartition.AddOrUpdate(
                partitionId,
                totalMessages,
                (_, count) => count + totalMessages);

            // Track processing time for this partition
            _processingTimeByPartition.AddOrUpdate(
                partitionId,
                processingTime,
                (_, time) => time + processingTime);
        }
    }

    private async Task ProcessPartitionMessagesAsync(
        IEnumerable<ConsumeResult<TKey, TValue>> partitionMessages,
        TopicPartition partition,
        Dictionary<int, (int TotalMessages, int ProcessedCount, long ProcessingTime)> partitionMetrics)
    {
        var stopwatch = Stopwatch.StartNew();
        var partitionId = partition.Partition.Value;
        var processedCount = 0;
        var totalMessages = 0;

        try
        {
            var maxOffset = 0L;
            var results = partitionMessages as ConsumeResult<TKey, TValue>[] ?? partitionMessages.ToArray();
            var initialCapacity = Math.Min(results.Length * 2, 1024);
            var latestMessageBySymbol =
                new Dictionary<TKey, (ConsumeResult<TKey, TValue> Message, long SequenceNumber)>(initialCapacity);

            foreach (var result in results)
            {
                totalMessages++;

                // ติดตาม offset สูงสุด
                var resultOffset = result.Offset;
                if (resultOffset > maxOffset)
                    maxOffset = resultOffset;

                var key = result.Message.Key;
                var sequenceNumber = GetSequenceNumber(result.Message.Headers);

                if (!latestMessageBySymbol.TryGetValue(key, out var existing) ||
                    sequenceNumber > existing.SequenceNumber)
                    latestMessageBySymbol[key] = (result, sequenceNumber);
            }

            _pendingOffsets[partition] = new TopicPartitionOffset(partition, maxOffset + 1);

            processedCount = await ProcessMessagesWithParallelAsync(latestMessageBySymbol.Values);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process latest messages in partition {Partition}", partition);
        }

        stopwatch.Stop();
        partitionMetrics[partitionId] = (totalMessages, processedCount, stopwatch.ElapsedMilliseconds);
    }

    private async Task<int> ProcessMessagesWithParallelAsync(
        IEnumerable<(ConsumeResult<TKey, TValue> Message, long SequenceNumber)> messages)
    {
        var localProcessedCount = 0;
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        await Parallel.ForEachAsync(
            messages,
            parallelOptions,
            async (item, _) =>
            {
                await _messageHandler.HandleAsync(item.Message.Message.Value);
                Interlocked.Increment(ref localProcessedCount);
            });

        return localProcessedCount;
    }

    private static long GetSequenceNumber(Headers headers)
    {
        if (headers.Count > 2 && headers[2].Key == SequenceNumberKey)
        {
            ReadOnlySpan<byte> valueBytes = headers[2].GetValueBytes();
            return valueBytes.Length == 8 ? BitConverter.ToInt64(valueBytes) : 0;
        }

        foreach (var header in headers)
            if (header.Key == SequenceNumberKey)
            {
                ReadOnlySpan<byte> bytes = header.GetValueBytes();
                return bytes.Length == 8 ? BitConverter.ToInt64(bytes) : 0;
            }

        return 0;
    }

    private async Task PerformanceMetricsLogger()
    {
        while (!_disposed)
        {
            await Task.Delay(TimeSpan.FromMinutes(3));

            if (_disposed) break;

            var totalMessages = Interlocked.Read(ref _totalMessagesConsumed);
            var uptime = _uptimeStopwatch.Elapsed;
            var lastMinuteMsg = Interlocked.Read(ref _lastMinuteMessages);
            var processedMessages = Interlocked.Read(ref _totalProcessedMessages);
            var skippedMessages = Interlocked.Read(ref _totalMessagesSkipped);

            // Optimization ratio calculation
            var optimizationRatio = totalMessages > 0
                ? (double)skippedMessages / totalMessages * 100
                : 0;

            _logger.LogInformation(
                "Performance metrics: Uptime={Uptime}, Total Messages={Total}, Processed={Processed}, Skipped={Skipped}, " +
                "Optimization={OptimizationRatio}%, Recent Rate={RecentRate} msg/min, Overall Rate={OverallRate} msg/sec, " +
                "Batches={Batches}, Commits={Commits}",
                uptime.ToString(@"d\.hh\:mm\:ss"),
                totalMessages,
                processedMessages,
                skippedMessages,
                optimizationRatio.ToString("F2"),
                lastMinuteMsg,
                uptime.TotalSeconds > 0 ? (totalMessages / uptime.TotalSeconds).ToString("F2") : "0",
                Interlocked.Read(ref _totalBatchesProcessed),
                Interlocked.Read(ref _totalCommits)
            );

            // Create a snapshot of the partition metrics
            var partitionMetrics = _messagesByPartition
                .OrderByDescending(kv => kv.Value)
                .Select(kv => new
                {
                    Partition = kv.Key,
                    Messages = kv.Value,
                    ProcessingTime = _processingTimeByPartition.GetValueOrDefault(kv.Key, 0),
                    Percentage = totalMessages > 0
                        ? (double)kv.Value / totalMessages * 100
                        : 0
                })
                .Take(5); // Top 5 partitions

            // Log partition metrics
            foreach (var metric in partitionMetrics)
                _logger.LogInformation(
                    "Partition {Partition}: {Messages} messages ({Percentage}%), Avg processing time: {AvgTime}ms",
                    metric.Partition,
                    metric.Messages,
                    metric.Percentage.ToString("F2"),
                    metric.Messages > 0 ? (metric.ProcessingTime / metric.Messages).ToString("F2") : "0"
                );
        }
    }

    private void CommitOffsets()
    {
        var commitStopwatch = Stopwatch.StartNew();
        try
        {
            if (_consumer != null && !_pendingOffsets.IsEmpty)
            {
                _consumer.Commit(_pendingOffsets.Values);
                Interlocked.Increment(ref _totalCommits);
                _lastCommitTime = DateTime.UtcNow;

                commitStopwatch.Stop();
                _logger.LogInformation("Committed {Count} offsets across {PartitionCount} partitions in {ElapsedMs}ms",
                    _pendingOffsets.Count, _pendingOffsets.Keys.Count, commitStopwatch.ElapsedMilliseconds);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit offsets");
        }
    }

    private void ValidateConfiguration()
    {
        if (_batchSize <= 0) _batchSize = 100;
        if (_maxBatchProcessingTimeMs <= 0) _maxBatchProcessingTimeMs = 1000;

        // Log the configuration
        _logger.LogInformation(
            "Kafka consumer configuration: BatchSize={BatchSize}, MaxBatchProcessingTimeMs={MaxProcessingTime}, " +
            "CommitIntervalMs={CommitInterval}",
            _batchSize, _maxBatchProcessingTimeMs, _commitIntervalMs);
    }

    private void StartPerformanceMetricsLogger()
    {
        Task.Run(async () => { await PerformanceMetricsLogger(); });
    }
}