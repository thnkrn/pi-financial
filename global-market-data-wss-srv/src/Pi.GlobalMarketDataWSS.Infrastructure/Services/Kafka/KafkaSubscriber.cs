using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketDataWSS.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Kafka;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Services.Kafka;

public sealed class KafkaSubscriber<TKey, TValue> : IKafkaSubscriber, IDisposable where TKey : notnull
{
    private const string HighVolumeTopicTopic = "ge_stock_market_non_trade_data";
    private const string ManualCommitTopic = "ge_stock_market_price_status";
    
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaSubscriber<TKey, TValue>> _logger;
    private readonly IKafkaMessageHandler<Message<TKey, TValue>> _messageHandler;
    private readonly string? _topic;
    private readonly CancellationTokenSource _internalCts = new();
    
    // Performance configuration
    private readonly ConsumerConfiguration _consumerConfig;
    private readonly bool _enableDeduplication;
    private readonly bool _enableManualCommit;
    
    // High-performance channel for message processing
    private readonly Channel<ProcessingItem> _processingChannel;
    private readonly Task[] _processingTasks;
    
    // Offset tracking for manual commits
    private readonly ConcurrentDictionary<TopicPartition, long> _pendingOffsets = new();
    private readonly Timer? _commitTimer;
    
    // Performance metrics
    private readonly PerformanceMetrics _metrics = new();
    private readonly Timer? _metricsTimer;
    private readonly Stopwatch _uptimeStopwatch = Stopwatch.StartNew();
    
    // Consumer state
    private IConsumer<TKey, TValue>? _consumer;
    private bool _disposed;
    private DateTime _lastCommitTime = DateTime.UtcNow;
    
    // Deduplication cache (only if enabled)
    private readonly ConcurrentDictionary<string, long>? _deduplicationCache;

    private readonly struct ProcessingItem
    {
        public Message<TKey, TValue> Message { get; init; }
        public TopicPartition Partition { get; init; }
        public long Offset { get; init; }
    }

    private class PerformanceMetrics
    {
        public long TotalMessagesReceived;
        public long TotalMessagesProcessed;
        public long TotalMessagesSkipped;
        public long ProcessingErrors;
        public long CommitCount;
    }

    private class ConsumerConfiguration
    {
        public int ChannelCapacity { get; init; }
        public int MaxConcurrentTasks { get; init; }
        public int BatchSize { get; init; }
        public int BatchTimeoutMs { get; init; }
        public int CommitIntervalMs { get; init; }
        public int MetricsIntervalSeconds { get; init; }
    }

    public KafkaSubscriber(
        IConfiguration configuration,
        string? topic,
        IKafkaMessageHandler<Message<TKey, TValue>> messageHandler,
        ILogger<KafkaSubscriber<TKey, TValue>> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _topic = topic;
        _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Load configuration
        _consumerConfig = LoadConfiguration();
        _enableManualCommit = string.Equals(_topic, ManualCommitTopic, StringComparison.OrdinalIgnoreCase);
        _enableDeduplication = _configuration.GetValue("KAFKA:ENABLE_DEDUPLICATION", false);

        // Initialize deduplication cache if enabled
        if (_enableDeduplication)
        {
            _deduplicationCache = new ConcurrentDictionary<string, long>();
        }

        // Create high-performance channel
        _processingChannel = Channel.CreateBounded<ProcessingItem>(new BoundedChannelOptions(_consumerConfig.ChannelCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = false
        });

        // Start processing tasks
        _processingTasks = new Task[_consumerConfig.MaxConcurrentTasks];
        for (int i = 0; i < _processingTasks.Length; i++)
        {
            _processingTasks[i] = Task.Run(() => ProcessMessagesAsync(_internalCts.Token));
        }

        // Setup timers
        if (_enableManualCommit)
        {
            _commitTimer = new Timer(
                CommitPendingOffsets,
                null,
                TimeSpan.FromMilliseconds(_consumerConfig.CommitIntervalMs),
                TimeSpan.FromMilliseconds(_consumerConfig.CommitIntervalMs));
        }

        _metricsTimer = new Timer(
            LogMetrics,
            null,
            TimeSpan.FromSeconds(_consumerConfig.MetricsIntervalSeconds),
            TimeSpan.FromSeconds(_consumerConfig.MetricsIntervalSeconds));

        _logger.LogInformation(
            "Initialized high-performance Kafka subscriber: Topic={Topic}, Concurrency={Concurrency}, " +
            "ChannelCapacity={Capacity}, Deduplication={Dedup}, ManualCommit={Manual}",
            _topic, _consumerConfig.MaxConcurrentTasks, _consumerConfig.ChannelCapacity,
            _enableDeduplication, _enableManualCommit);
    }

    private ConsumerConfiguration LoadConfiguration()
    {
        var isHighVolumeTopic = !string.IsNullOrEmpty(_topic) && 
                               _topic.Equals(HighVolumeTopicTopic, StringComparison.OrdinalIgnoreCase);

        return new ConsumerConfiguration
        {
            ChannelCapacity = _configuration.GetValue("KAFKA:CHANNEL_CAPACITY", 100000),
            MaxConcurrentTasks = _configuration.GetValue("KAFKA:MAX_CONCURRENT_TASKS", 
                isHighVolumeTopic ? 50 : 100),
            BatchSize = _configuration.GetValue("KAFKA:BATCH_SIZE", 1000),
            BatchTimeoutMs = _configuration.GetValue("KAFKA:BATCH_TIMEOUT_MS", 50),
            CommitIntervalMs = _configuration.GetValue("KAFKA:COMMIT_INTERVAL_MS", 1000),
            MetricsIntervalSeconds = _configuration.GetValue("KAFKA:METRICS_INTERVAL_SECONDS", 60)
        };
    }

    public async Task SubscribeAsync(CancellationToken cancellationToken)
    {
        var config = CreateConsumerConfig();
        
        try
        {
            _consumer = new ConsumerBuilder<TKey, TValue>(config)
                .SetErrorHandler(HandleError)
                .SetPartitionsAssignedHandler(HandlePartitionsAssigned)
                .SetPartitionsRevokedHandler(HandlePartitionsRevoked)
                .SetStatisticsHandler(HandleStatistics)
                .Build();

            _consumer.Subscribe(_topic);
            _logger.LogInformation("Successfully subscribed to Kafka topic: {Topic}", _topic);

            using var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _internalCts.Token);
            await ConsumeLoopAsync(linkedToken.Token);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to subscribe to Kafka topic {Topic}", _topic);
            throw;
        }
    }

    public async Task UnsubscribeAsync()
    {
        try
        {
            await _internalCts.CancelAsync();
            
            // Complete channel
            _processingChannel.Writer.TryComplete();
            
            // Wait for processing to complete
            await Task.WhenAll(_processingTasks).WaitAsync(TimeSpan.FromSeconds(30));
            
            // Final commit if needed
            if (_enableManualCommit)
            {
                CommitPendingOffsets(null);
            }
            
            _consumer?.Close();
            _logger.LogInformation("Successfully unsubscribed from Kafka topic: {Topic}", _topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during unsubscribe");
            throw;
        }
    }

    private ConsumerConfig CreateConsumerConfig()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration[ConfigurationKeys.KafkaBootstrapServers],
            GroupId = _configuration[ConfigurationKeys.KafkaConsumerGroupId],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = !_enableManualCommit,
            
            // Optimized for high throughput
            FetchMinBytes = _configuration.GetValue("KAFKA:FETCH_MIN_BYTES", 1048576), // 1MB
            FetchWaitMaxMs = _configuration.GetValue("KAFKA:FETCH_WAIT_MAX_MS", 100),
            FetchMaxBytes = _configuration.GetValue("KAFKA:FETCH_MAX_BYTES", 104857600), // 100MB
            MaxPartitionFetchBytes = _configuration.GetValue("KAFKA:MAX_PARTITION_FETCH_BYTES", 52428800), // 50MB
            
            // Large internal queues
            QueuedMinMessages = _configuration.GetValue("KAFKA:QUEUED_MIN_MESSAGES", 100000),
            QueuedMaxMessagesKbytes = _configuration.GetValue("KAFKA:QUEUED_MAX_MESSAGES_KBYTES", 2097151),  // 2GB
            
            // Session settings
            SessionTimeoutMs = _configuration.GetValue("KAFKA:SESSION_TIMEOUT_MS", 45000),
            MaxPollIntervalMs = _configuration.GetValue("KAFKA:MAX_POLL_INTERVAL_MS", 600000),
            HeartbeatIntervalMs = _configuration.GetValue("KAFKA:HEARTBEAT_INTERVAL_MS", 3000),
            
            // Network optimization
            SocketKeepaliveEnable = true,
            SocketTimeoutMs = 60000,
            SocketReceiveBufferBytes = 8388608, // 8MB
            
            // Use cooperative sticky for better rebalancing
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky,
            
            // Statistics for monitoring
            StatisticsIntervalMs = 30000
        };

        ConfigureSecurity(config);
        return config;
    }

    private void ConfigureSecurity(ConsumerConfig config)
    {
        try
        {
            var securityProtocolString = _configuration[ConfigurationKeys.KafkaSecurityProtocol] ?? "SASL_SSL";
            config.SecurityProtocol = Enum.TryParse<SecurityProtocol>(
                securityProtocolString.Replace("_", string.Empty), true, out var securityProtocol)
                ? securityProtocol : SecurityProtocol.SaslSsl;

            var saslMechanismString = _configuration[ConfigurationKeys.KafkaSaslMechanism] ?? "PLAIN";
            config.SaslMechanism = Enum.TryParse<SaslMechanism>(
                saslMechanismString.Replace("_", string.Empty), true, out var saslMechanism)
                ? saslMechanism : SaslMechanism.Plain;

            config.SaslUsername = _configuration[ConfigurationKeys.KafkaSaslUsername];
            config.SaslPassword = _configuration[ConfigurationKeys.KafkaSaslPassword];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error configuring Kafka security settings");
            throw;
        }
    }

    private async Task ConsumeLoopAsync(CancellationToken cancellationToken)
    {
        var consecutiveErrors = 0;
        const int maxConsecutiveErrors = 10;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ConsumeMessagesAsync(cancellationToken);
                consecutiveErrors = 0;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                consecutiveErrors++;
                _logger.LogError(ex, "Error in consume loop (attempt {Count}/{Max})", 
                    consecutiveErrors, maxConsecutiveErrors);
                
                if (consecutiveErrors >= maxConsecutiveErrors)
                {
                    _logger.LogError("Max consecutive errors reached. Exiting consume loop.");
                    throw;
                }
                
                await Task.Delay(TimeSpan.FromSeconds(Math.Min(consecutiveErrors, 10)), cancellationToken);
            }
        }
    }

    private async Task ConsumeMessagesAsync(CancellationToken cancellationToken)
    {
        var batchStopwatch = Stopwatch.StartNew();
        var messagesInBatch = 0;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Consume with zero timeout for non-blocking
                var consumeResult = _consumer?.Consume(TimeSpan.Zero);
                
                if (consumeResult != null && !consumeResult.IsPartitionEOF)
                {
                    Interlocked.Increment(ref _metrics.TotalMessagesReceived);
                    messagesInBatch++;
                    
                    // Check deduplication if enabled
                    if (_enableDeduplication && ShouldSkipMessage(consumeResult))
                    {
                        Interlocked.Increment(ref _metrics.TotalMessagesSkipped);
                        TrackOffset(consumeResult);
                        continue;
                    }
                    
                    // Create processing item
                    var item = new ProcessingItem
                    {
                        Message = consumeResult.Message,
                        Partition = consumeResult.TopicPartition,
                        Offset = consumeResult.Offset.Value
                    };
                    
                    // Try to write to channel without blocking
                    if (!_processingChannel.Writer.TryWrite(item))
                    {
                        // Channel is full, write async
                        await _processingChannel.Writer.WriteAsync(item, cancellationToken);
                    }
                    
                    // Track offset for manual commit
                    TrackOffset(consumeResult);
                    
                    // Check if we should pause consumption
                    if (_processingChannel.Reader.Count > _consumerConfig.ChannelCapacity * 0.9)
                    {
                        _logger.LogDebug("Channel near capacity ({Count}/{Capacity}), pausing consumption",
                            _processingChannel.Reader.Count, _consumerConfig.ChannelCapacity);
                        await Task.Delay(10, cancellationToken);
                    }
                }
                else
                {
                    // No message available
                    if (messagesInBatch == 0)
                    {
                        // Prevent CPU spinning
                        await Task.Delay(10, cancellationToken);
                    }
                }
                
                // Check batch completion conditions
                if (messagesInBatch >= _consumerConfig.BatchSize ||
                    (messagesInBatch > 0 && batchStopwatch.ElapsedMilliseconds >= _consumerConfig.BatchTimeoutMs))
                {
                    _logger.LogDebug("Batch complete: {Count} messages in {ElapsedMs}ms",
                        messagesInBatch, batchStopwatch.ElapsedMilliseconds);
                    
                    messagesInBatch = 0;
                    batchStopwatch.Restart();
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Consume error: {Reason}", ex.Error.Reason);
                throw;
            }
        }
    }

    private bool ShouldSkipMessage(ConsumeResult<TKey, TValue> consumeResult)
    {
        if (!_enableDeduplication || _deduplicationCache == null)
            return false;
        
        var key = consumeResult.Message.Key?.ToString();
        if (string.IsNullOrEmpty(key))
            return false;
        
        var currentOffset = consumeResult.Offset.Value;
        
        // Check if we've seen a newer message for this key
        if (_deduplicationCache.TryGetValue(key, out var existingOffset))
        {
            if (existingOffset >= currentOffset)
                return true; // Skip older message
        }
        
        // Update cache with newer offset
        _deduplicationCache.AddOrUpdate(key, currentOffset, (_, old) => Math.Max(old, currentOffset));
        
        // Periodically clean up old entries (simple strategy)
        if (_deduplicationCache.Count > 100000)
        {
            Task.Run(() => CleanupDeduplicationCache());
        }
        
        return false;
    }

    private void CleanupDeduplicationCache()
    {
        if (_deduplicationCache == null || _deduplicationCache.Count < 100000)
            return;
        
        try
        {
            // Remove oldest 20% of entries
            var entriesToRemove = _deduplicationCache
                .OrderBy(kvp => kvp.Value)
                .Take(_deduplicationCache.Count / 5)
                .Select(kvp => kvp.Key)
                .ToList();
            
            foreach (var key in entriesToRemove)
            {
                _deduplicationCache.TryRemove(key, out _);
            }
            
            _logger.LogDebug("Cleaned up {Count} entries from deduplication cache", entriesToRemove.Count);
        }
        catch (InvalidOperationException ex)
        {
            // Collection was modified during enumeration
            _logger.LogWarning(ex, "Deduplication cache was modified during cleanup, will retry next time");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error cleaning up deduplication cache");
        }
    }

    private async Task ProcessMessagesAsync(CancellationToken cancellationToken)
    {
        await foreach (var item in _processingChannel.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                await _messageHandler.HandleAsync(item.Message);
                Interlocked.Increment(ref _metrics.TotalMessagesProcessed);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _metrics.ProcessingErrors);
                _logger.LogError(ex, "Error processing message from partition {Partition}, offset {Offset}",
                    item.Partition, item.Offset);
            }
        }
    }

    private void TrackOffset(ConsumeResult<TKey, TValue> consumeResult)
    {
        if (!_enableManualCommit)
            return;
        
        _pendingOffsets.AddOrUpdate(
            consumeResult.TopicPartition,
            consumeResult.Offset.Value,
            (_, existing) => Math.Max(existing, consumeResult.Offset.Value));
    }

    private void CommitPendingOffsets(object? state)
    {
        if (!_enableManualCommit || _consumer == null || _pendingOffsets.IsEmpty || _disposed)
            return;
        
        try
        {
            var offsetsToCommit = _pendingOffsets
                .ToArray()
                .Select(kvp => new TopicPartitionOffset(kvp.Key, kvp.Value + 1))
                .ToList();
            
            if (offsetsToCommit.Count == 0)
                return;
            
            _consumer.Commit(offsetsToCommit);
            
            // Clear committed offsets
            foreach (var offset in offsetsToCommit)
            {
                _pendingOffsets.TryRemove(offset.TopicPartition, out _);
            }
            
            Interlocked.Increment(ref _metrics.CommitCount);
            _lastCommitTime = DateTime.UtcNow;
            
            _logger.LogDebug("Committed {Count} offsets", offsetsToCommit.Count);
        }
        catch (KafkaException ex) when (ex.Error.Code == ErrorCode.RebalanceInProgress)
        {
            _logger.LogDebug("Cannot commit offsets during rebalance, will retry later");
        }
        catch (KafkaException ex) when (ex.Error.Code == ErrorCode.UnknownMemberId || 
                                       ex.Error.Code == ErrorCode.IllegalGeneration)
        {
            _logger.LogWarning("Consumer group membership changed, skipping offset commit");
        }
        catch (ObjectDisposedException)
        {
            _logger.LogDebug("Consumer was disposed while committing offsets");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error committing offsets");
        }
    }

    private void HandleError(IConsumer<TKey, TValue> consumer, Error error)
    {
        _logger.LogError("Kafka error: {Reason} (Code: {Code})", error.Reason, error.Code);
        
        if (error.IsFatal)
        {
            _logger.LogError("Fatal Kafka error detected. Consumer needs restart.");
        }
    }

    private void HandlePartitionsAssigned(IConsumer<TKey, TValue> consumer, List<TopicPartition> partitions)
    {
        _logger.LogInformation("Assigned partitions: [{Partitions}]",
            string.Join(", ", partitions.Select(p => p.Partition.Value)));
    }

    private void HandlePartitionsRevoked(IConsumer<TKey, TValue> consumer, List<TopicPartitionOffset> partitions)
    {
        _logger.LogInformation("Revoking partitions: [{Partitions}]",
            string.Join(", ", partitions.Select(p => p.Partition.Value)));
        
        // Commit any pending offsets before rebalance
        if (_enableManualCommit)
        {
            CommitPendingOffsets(null);
        }
    }

    private void HandleStatistics(IConsumer<TKey, TValue> consumer, string json)
    {
        // Parse important stats if needed
        _logger.LogDebug("Kafka statistics received");
    }

    private void LogMetrics(object? state)
    {
        var received = Interlocked.Read(ref _metrics.TotalMessagesReceived);
        var processed = Interlocked.Read(ref _metrics.TotalMessagesProcessed);
        var skipped = Interlocked.Read(ref _metrics.TotalMessagesSkipped);
        var errors = Interlocked.Read(ref _metrics.ProcessingErrors);
        var commits = Interlocked.Read(ref _metrics.CommitCount);
        
        var uptime = _uptimeStopwatch.Elapsed;
        var receiveRate = uptime.TotalSeconds > 0 ? received / uptime.TotalSeconds : 0;
        var processRate = uptime.TotalSeconds > 0 ? processed / uptime.TotalSeconds : 0;
        
        _logger.LogInformation(
            "Metrics: Received={Received:N0} ({ReceiveRate:F1}/s), Processed={Processed:N0} ({ProcessRate:F1}/s), " +
            "Skipped={Skipped:N0}, Errors={Errors:N0}, Commits={Commits:N0}, ChannelSize={ChannelSize}, " +
            "Uptime={Uptime}",
            received, receiveRate, processed, processRate, skipped, errors, commits,
            _processingChannel.Reader.Count, uptime.ToString(@"d\.hh\:mm\:ss"));
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;
        
        try
        {
            _internalCts.Cancel();
            _processingChannel.Writer.TryComplete();
            
            try
            {
                Task.WaitAll(_processingTasks, TimeSpan.FromSeconds(30));
            }
            catch (AggregateException ex)
            {
                _logger.LogWarning(ex, "Some processing tasks did not complete within timeout during disposal");
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation token is triggered
                _logger.LogDebug("Processing tasks were cancelled during disposal");
            }
            
            _commitTimer?.Dispose();
            _metricsTimer?.Dispose();
            
            try
            {
                if (_consumer != null)
                {
                    _consumer.Close();
                    _consumer.Dispose();
                }
            }
            catch (ObjectDisposedException)
            {
                // Consumer was already disposed
                _logger.LogDebug("Consumer was already disposed");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error occurred while closing Kafka consumer during disposal");
            }
            
            _internalCts.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during KafkaSubscriber disposal");
        }
    }
}