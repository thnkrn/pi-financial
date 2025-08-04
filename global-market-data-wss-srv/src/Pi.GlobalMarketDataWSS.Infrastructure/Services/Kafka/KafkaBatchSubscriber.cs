using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketDataWSS.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataWSS.Infrastructure.Extensions;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Kafka;
using Polly;
using Polly.Retry;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Services.Kafka;

public sealed class KafkaBatchSubscriber<TKey, TValue> : IKafkaBatchSubscriber, IDisposable where TKey : notnull
{
    private readonly IConfiguration _configuration;
    private readonly CancellationTokenSource _internalCts = new();
    private readonly ILogger<KafkaBatchSubscriber<TKey, TValue>> _logger;
    private readonly IKafkaMessageHandler<TValue> _messageHandler;
    private readonly ConcurrentDictionary<int, long> _messagesByPartition = new();
    private readonly ConcurrentDictionary<TopicPartition, TopicPartitionOffset> _pendingOffsets = new();
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly string _topic;

    // Performance monitoring
    private readonly Stopwatch _uptimeStopwatch = Stopwatch.StartNew();
    private readonly PerformanceCounters _performanceCounters = new();
    
    // Configuration
    private readonly ConsumerConfiguration _consumerConfig;
    
    // Message processing channel for better throughput
    private readonly Channel<ProcessingItem> _processingChannel;
    private readonly Task[] _processingTasks;

    private IConsumer<TKey, TValue>? _consumer;
    private bool _disposed;
    private bool _isSubscribed;

    // Structure for channel processing
    private readonly struct ProcessingItem
    {
        public TValue Value { get; init; }
        public TopicPartition Partition { get; init; }
        public long Offset { get; init; }
    }

    // Performance counters structure
    private sealed class PerformanceCounters
    {
        public long TotalMessagesConsumed;
        public long TotalMessagesProcessed;
        public long TotalBatchesProcessed;
        public long TotalCommits;
        public long ProcessingErrors;
    }

    // Consumer configuration structure
    private sealed class ConsumerConfiguration
    {
        public int BatchSize { get; init; }
        public int MaxBatchProcessingTimeMs { get; init; }
        public int CommitIntervalMs { get; init; }
        public int MaxRetryAttempts { get; init; }
        public int RetryDelayMs { get; init; }
        public int ParallelismFactor { get; init; }
        public int ChannelCapacity { get; init; }
        public bool EnableCatchUpMode { get; init; }
        public int CatchUpBatchSize { get; init; }
    }

    public KafkaBatchSubscriber(
        IConfiguration configuration,
        string topic,
        IKafkaMessageHandler<TValue> messageHandler,
        ILogger<KafkaBatchSubscriber<TKey, TValue>> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _topic = topic ?? throw new ArgumentNullException(nameof(topic));
        _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Load optimized configuration
        _consumerConfig = LoadConfiguration();

        // Create bounded channel for processing
        _processingChannel = Channel.CreateBounded<ProcessingItem>(new BoundedChannelOptions(_consumerConfig.ChannelCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = false
        });

        // Start processing tasks
        _processingTasks = new Task[_consumerConfig.ParallelismFactor];
        for (int i = 0; i < _processingTasks.Length; i++)
        {
            _processingTasks[i] = Task.Run(() => ProcessMessagesFromChannelAsync(_internalCts.Token));
        }

        // Create retry policy
        _retryPolicy = CreateRetryPolicy();

        // Start performance monitoring
        StartPerformanceMonitoring();
    }

    private ConsumerConfiguration LoadConfiguration()
    {
        return new ConsumerConfiguration
        {
            BatchSize = _configuration.GetValue("KAFKA:CONSUMER_BATCH_SIZE", 5000),
            MaxBatchProcessingTimeMs = _configuration.GetValue("KAFKA:CONSUMER_MAX_BATCH_PROCESSING_TIME_MS", 50),
            CommitIntervalMs = _configuration.GetValue("KAFKA:CONSUMER_COMMIT_INTERVAL_MS", 500),
            MaxRetryAttempts = _configuration.GetValue("KAFKA:CONSUMER_MAX_RETRY_ATTEMPTS", 5),
            RetryDelayMs = _configuration.GetValue("KAFKA:CONSUMER_RETRY_DELAY_MS", 1000),
            ParallelismFactor = _configuration.GetValue("KAFKA:PARALLELISM_FACTOR", Environment.ProcessorCount * 4),
            ChannelCapacity = _configuration.GetValue("KAFKA:CHANNEL_CAPACITY", 500000),
            EnableCatchUpMode = _configuration.GetValue("KAFKA:ENABLE_CATCH_UP_MODE", true),
            CatchUpBatchSize = _configuration.GetValue("KAFKA:CATCH_UP_BATCH_SIZE", 20000)
        };
    }

    private AsyncRetryPolicy CreateRetryPolicy()
    {
        return Policy
            .Handle<KafkaException>()
            .Or<ConsumeException>()
            .WaitAndRetryAsync(
                _consumerConfig.MaxRetryAttempts,
                retryAttempt => TimeSpan.FromMilliseconds(_consumerConfig.RetryDelayMs * Math.Pow(2, retryAttempt - 1)),
                (ex, timeSpan, retryCount, _) =>
                {
                    if (retryCount == _consumerConfig.MaxRetryAttempts)
                        _logger.LogError(ex, "Kafka connection failed after {MaxRetries} attempts", _consumerConfig.MaxRetryAttempts);
                    else
                        _logger.LogWarning(ex, "Retrying Kafka connection in {DelayMs}ms", timeSpan.TotalMilliseconds);
                });
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _internalCts.Cancel();
        
        // Wait for channel to complete
        _processingChannel.Writer.TryComplete();
        Task.WaitAll(_processingTasks, TimeSpan.FromSeconds(30));
        
        // Close consumer
        _consumer?.Close();
        _consumer?.Dispose();
        
        _internalCts.Dispose();
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
                    .SetErrorHandler(HandleKafkaError)
                    .SetPartitionsAssignedHandler(HandlePartitionsAssigned)
                    .SetPartitionsRevokedHandler(HandlePartitionsRevoked)
                    .SetStatisticsHandler(HandleStatistics)
                    .Build();

                _consumer.Subscribe(_topic);
                _isSubscribed = true;
                _logger.LogInformation("Successfully subscribed to Kafka topic: {Topic}", _topic);

                using var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _internalCts.Token);
                await ConsumeLoopAsync(linkedToken.Token);
            });
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to subscribe to Kafka topic {Topic}", _topic);
            throw;
        }
    }

    public async Task UnsubscribeAsync()
    {
        if (!_isSubscribed)
        {
            _logger.LogDebug("No active subscription to unsubscribe from");
            return;
        }

        try
        {
            await _internalCts.CancelAsync();
            
            // Final commit
            await CommitPendingOffsetsAsync();
            
            _consumer?.Unsubscribe();
            _isSubscribed = false;
            _logger.LogInformation("Successfully unsubscribed from Kafka topic: {Topic}", _topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while unsubscribing from Kafka topic: {Topic}", _topic);
            throw;
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
            
            // Optimized for maximum throughput
            FetchMinBytes = _configuration.GetValue("KAFKA:CONSUMER_FETCH_MIN_BYTES", 1048576), // 1MB
            FetchWaitMaxMs = _configuration.GetValue("KAFKA:CONSUMER_FETCH_WAIT_MAX_MS", 100),
            SessionTimeoutMs = _configuration.GetValue("KAFKA:CONSUMER_SESSION_TIMEOUT_MS", 45000),
            MaxPollIntervalMs = _configuration.GetValue("KAFKA:CONSUMER_MAX_POLL_INTERVAL_MS", 600000), // 10 minutes
            MaxPartitionFetchBytes = _configuration.GetValue("KAFKA:CONSUMER_MAX_PARTITION_FETCH_BYTES", 4194304), // 100MB
            FetchMaxBytes = _configuration.GetValue("KAFKA:CONSUMER_FETCH_MAX_BYTES", 1073741824), // 1GB
            QueuedMinMessages = _configuration.GetValue("KAFKA:CONSUMER_QUEUED_MIN_MESSAGES", 1000000),
            QueuedMaxMessagesKbytes = _configuration.GetValue("KAFKA:CONSUMER_QUEUED_MAX_MESSAGES_KBYTES", 1048576),
            
            // Performance settings
            AllowAutoCreateTopics = false,
            EnablePartitionEof = false,
            SocketTimeoutMs = 60000,
            SocketKeepaliveEnable = true,
            SocketReceiveBufferBytes = 16777216, // 16MB
            StatisticsIntervalMs = 30000, // Every 30 seconds
        };

        // Security settings
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
        var catchUpMode = _consumerConfig.EnableCatchUpMode;
        var consecutiveEmptyBatches = 0;
        var lastStatsLog = DateTime.UtcNow;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Determine batch size based on mode
                var currentBatchSize = catchUpMode ? _consumerConfig.CatchUpBatchSize : _consumerConfig.BatchSize;
                var batchProcessed = await ConsumeBatchStreamAsync(currentBatchSize, cancellationToken);
                
                if (!batchProcessed)
                {
                    consecutiveEmptyBatches++;
                    if (consecutiveEmptyBatches > 10 && catchUpMode)
                    {
                        catchUpMode = false;
                        _logger.LogInformation("Switching from catch-up mode to normal mode");
                    }
                }
                else
                {
                    consecutiveEmptyBatches = 0;
                }
                
                // Periodic stats logging
                if ((DateTime.UtcNow - lastStatsLog).TotalSeconds >= 10)
                {
                    LogQuickStats();
                    lastStatsLog = DateTime.UtcNow;
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error in consume loop");
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    private async Task<bool> ConsumeBatchStreamAsync(int batchSize, CancellationToken cancellationToken)
    {
        var consumeStopwatch = Stopwatch.StartNew();
        var hasMessages = false;
        var messagesProcessed = 0;
        var offsetUpdates = new Dictionary<TopicPartition, long>();
        
        // Stream processing - process immediately without collecting
        while (messagesProcessed < batchSize && 
               consumeStopwatch.ElapsedMilliseconds < _consumerConfig.MaxBatchProcessingTimeMs)
        {
            try
            {
                var consumeResult = _consumer?.Consume(TimeSpan.Zero);
                if (consumeResult != null && !consumeResult.IsPartitionEOF)
                {
                    hasMessages = true;
                    messagesProcessed++;
                    
                    // Update offset tracking
                    offsetUpdates[consumeResult.TopicPartition] = consumeResult.Offset.Value;
                    
                    // Send to channel immediately
                    var item = new ProcessingItem
                    {
                        Value = consumeResult.Message.Value,
                        Partition = consumeResult.TopicPartition,
                        Offset = consumeResult.Offset.Value
                    };
                    
                    // Try to write without waiting if channel is not full
                    if (!_processingChannel.Writer.TryWrite(item))
                    {
                        // Channel is full, write async but don't block consume
                        _ = _processingChannel.Writer.WriteAsync(item, cancellationToken).AsTask();
                    }
                }
                else if (!hasMessages)
                {
                    // No messages available
                    break;
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Consume error");
                break;
            }
        }

        if (!hasMessages)
            return false;

        // Update counters and offsets
        Interlocked.Add(ref _performanceCounters.TotalMessagesConsumed, messagesProcessed);
        Interlocked.Increment(ref _performanceCounters.TotalBatchesProcessed);
        
        // Update pending offsets
        foreach (var kvp in offsetUpdates)
        {
            _pendingOffsets[kvp.Key] = new TopicPartitionOffset(kvp.Key, kvp.Value + 1);
        }

        // Handle commits
        await HandleCommitsAsync();
        
        return true;
    }

    private async Task ProcessMessagesFromChannelAsync(CancellationToken cancellationToken)
    {
        await foreach (var item in _processingChannel.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                await _messageHandler.HandleAsync(item.Value);
                Interlocked.Increment(ref _performanceCounters.TotalMessagesProcessed);
                
                // Update partition counter
                var partitionId = item.Partition.Partition.Value;
                _messagesByPartition.AddOrUpdate(partitionId, 1, (_, count) => count + 1);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _performanceCounters.ProcessingErrors);
                _logger.LogError(ex, "Error processing message from partition {Partition}", item.Partition);
            }
        }
    }

    private async Task HandleCommitsAsync()
    {
        if (_pendingOffsets.IsEmpty) return;

        var shouldCommit = _pendingOffsets.Count >= 100;

        if (shouldCommit)
        {
            await CommitPendingOffsetsAsync();
        }
    }

    private async Task CommitPendingOffsetsAsync()
    {
        if (_consumer == null || _pendingOffsets.IsEmpty) return;

        try
        {
            var offsetsToCommit = _pendingOffsets.ToArray()
                .Select(kvp => kvp.Value)
                .ToList();
            
            // Async commit for better performance
            await Task.Run(() => _consumer.Commit(offsetsToCommit));
            
            // Clear only committed offsets
            foreach (var offset in offsetsToCommit)
            {
                _pendingOffsets.TryRemove(offset.TopicPartition, out _);
            }
            
            Interlocked.Increment(ref _performanceCounters.TotalCommits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit offsets");
        }
    }

    // Event handlers
    private void HandleKafkaError(IConsumer<TKey, TValue> consumer, Error error)
    {
        _logger.LogError("Kafka error: {Reason} (Code: {Code})", error.Reason, error.Code);
        
        if (error.IsFatal)
        {
            _logger.LogWarning("Fatal Kafka error detected. Consumer will be restarted");
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                await RestartConsumerAsync();
            });
        }
    }

    private void HandlePartitionsAssigned(IConsumer<TKey, TValue> consumer, List<TopicPartition> partitions)
    {
        var partitionList = string.Join(", ", partitions.Select(p => p.Partition.Value));
        _logger.LogInformation("Assigned partitions: [{Partitions}]", partitionList);
        
        foreach (var partition in partitions)
        {
            _messagesByPartition.TryAdd(partition.Partition.Value, 0);
        }
    }

    private void HandlePartitionsRevoked(IConsumer<TKey, TValue> consumer, List<TopicPartitionOffset> partitions)
    {
        _logger.LogInformation("Revoking partitions: [{Partitions}]", 
            string.Join(", ", partitions.Select(p => p.Partition.Value)));
        
        // Force commit before revocation
        Task.Run(async () => await CommitPendingOffsetsAsync()).Wait(TimeSpan.FromSeconds(5));
    }

    private void HandleStatistics(IConsumer<TKey, TValue> consumer, string json)
    {
        // Parse and log important stats
        try
        {
            // You can parse the JSON statistics here if needed
            _logger.LogDebug("Kafka statistics received");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling statistics");
        }
    }

    private async Task RestartConsumerAsync()
    {
        try
        {
            _consumer?.Close();
            _consumer?.Dispose();
            _consumer = null;
            _isSubscribed = false;

            await Task.Delay(2000);
            await SubscribeAsync(_internalCts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restart consumer");
        }
    }

    // Performance monitoring
    private void StartPerformanceMonitoring()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? 
                         Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        if (!string.IsNullOrEmpty(environment) && 
            !environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            Task.Run(() => PerformanceMonitoringLoopAsync(_internalCts.Token));
        }
    }

    private async Task PerformanceMonitoringLoopAsync(CancellationToken cancellationToken)
    {
        var lastTotalMessages = 0L;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            
            var totalMessages = Interlocked.Read(ref _performanceCounters.TotalMessagesConsumed);
            var processedMessages = Interlocked.Read(ref _performanceCounters.TotalMessagesProcessed);
            var errors = Interlocked.Read(ref _performanceCounters.ProcessingErrors);
            var commits = Interlocked.Read(ref _performanceCounters.TotalCommits);
            var batches = Interlocked.Read(ref _performanceCounters.TotalBatchesProcessed);
            
            var messagesThisMinute = totalMessages - lastTotalMessages;
            lastTotalMessages = totalMessages;
            
            var uptime = _uptimeStopwatch.Elapsed;
            var overallRate = uptime.TotalSeconds > 0 ? totalMessages / uptime.TotalSeconds : 0;
            
            _logger.LogInformation(
                "Performance: Uptime={Uptime}, Total={Total:N0}, Processed={Processed:N0}, " +
                "Errors={Errors:N0}, Rate={Rate:N0}/min ({OverallRate:F1}/sec), " +
                "Batches={Batches:N0}, Commits={Commits:N0}, ChannelSize={ChannelSize}",
                uptime.ToString(@"d\.hh\:mm\:ss"),
                totalMessages,
                processedMessages,
                errors,
                messagesThisMinute,
                overallRate,
                batches,
                commits,
                _processingChannel.Reader.Count
            );

            // Log top partitions
            var topPartitions = _messagesByPartition
                .OrderByDescending(kv => kv.Value)
                .Take(5)
                .Select(kv => $"P{kv.Key}:{kv.Value:N0}");
                
            _logger.LogInformation("Top partitions: {Partitions}", string.Join(", ", topPartitions));
        }
    }

    private void LogQuickStats()
    {
        var consumed = Interlocked.Read(ref _performanceCounters.TotalMessagesConsumed);
        var processed = Interlocked.Read(ref _performanceCounters.TotalMessagesProcessed);
        var channelSize = _processingChannel.Reader.Count;
        
        _logger.LogInformation(
            "Quick stats: Consumed={Consumed:N0}, Processed={Processed:N0}, Channel={Channel}, ProcessingLag={Lag:N0}",
            consumed, processed, channelSize, consumed - processed
        );
    }
}