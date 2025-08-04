using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

public interface IKafkaProducerOptimizedService
{
    Task ProduceMessageAsync(Message<string, string> message, CancellationToken cancellationToken = default);
    Task ProduceBatchAsync(Message<string, string>?[] messages, CancellationToken cancellationToken = default);
    Task ProduceBatchToTopicAsync(Message<string, string>?[] messages, string topic, CancellationToken cancellationToken = default);
    
    ProducerMetricsSnapshot GetMetrics();
    void ResetMetrics();
    Task<bool> IsHealthy(CancellationToken cancellationToken = default);
}

public sealed class KafkaProducerOptimizedService : IKafkaProducerOptimizedService, IDisposable
{
    private const int MaxRetryCount = 3;
    private const int MaxBatchSizeBytes = 512_000; // 512 KB
    private const int MaxBatchSizeMessages = 1000;
    private readonly CircuitBreaker _circuitBreaker;
    private readonly ILogger<KafkaProducerOptimizedService> _logger;
    private readonly ProducerMetrics _metrics;
    private readonly Timer _metricsReportingTimer;
    private readonly IProducer<string, string> _producer;
    private readonly ProducerConfig _producerConfig;
    private readonly DefaultObjectPool<IProducer<string, string>> _producerPool;
    private readonly SemaphoreSlim _throttle;
    private readonly string _topic;
    private bool _disposed;

    public KafkaProducerOptimizedService(IConfiguration configuration, ILogger<KafkaProducerOptimizedService> logger)
    {
        _logger = logger;
        _metrics = new ProducerMetrics();

        // Max concurrent requests
        _throttle = new SemaphoreSlim(2000);
        _circuitBreaker = new CircuitBreaker(5, TimeSpan.FromSeconds(30));

        // Validate configuration before proceeding
        ValidateConfiguration(configuration);

        // Kafka configurations
        var bootstrapServers = configuration[ConfigurationKeys.KafkaBootstrapServers];

        _producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            SaslUsername = configuration[ConfigurationKeys.KafkaSaslUsername],
            SaslPassword = configuration[ConfigurationKeys.KafkaSaslPassword],

            // Performance Optimizations
            BatchSize = configuration.GetValue(ConfigurationKeys.ProducerConfigsBatchSize, 4194304), // 4MB batch size
            LingerMs = configuration.GetValue(ConfigurationKeys.ProducerConfigsLingerMs, 10), // 10ms linger
            CompressionType = CompressionType.Snappy,
            QueueBufferingMaxMessages =
                configuration.GetValue(ConfigurationKeys.producerConfigsQueueBufferingMaxMessages, 2000000),
            QueueBufferingMaxKbytes =
                configuration.GetValue(ConfigurationKeys.producerConfigsQueueBufferingMaxKBytes, 4194304), // 4MB
            MessageSendMaxRetries =
                configuration.GetValue(ConfigurationKeys.ProducerConfigsMessageSendMaxRetries, 2), // Reduced retries
            RetryBackoffMs =
                configuration.GetValue(ConfigurationKeys.ProducerConfigsRetryBackoffMs, 50), // Faster retry

            // Socket configurations
            SocketSendBufferBytes =
                configuration.GetValue(ConfigurationKeys.ProducerConfigsSocketSendBufferBytes, 1048576), // 1MB
            SocketReceiveBufferBytes =
                configuration.GetValue(ConfigurationKeys.ProducerConfigsSocketReceiveBufferBytes, 1048576), // 1MB

            // Connection Optimizations
            SocketTimeoutMs = configuration.GetValue(ConfigurationKeys.ProducerConfigsSocketTimeoutMs, 30000),
            MessageTimeoutMs = configuration.GetValue(ConfigurationKeys.ProducerConfigsMessageTimeoutMs, 30000),
            RequestTimeoutMs = configuration.GetValue(ConfigurationKeys.ProducerConfigsRequestTimeoutMs, 30000),
            MaxInFlight =
                configuration.GetValue(ConfigurationKeys.ProducerConfigsMaxInFlight, 3), // Reduced for better latency
            EnableIdempotence = configuration.GetValue(ConfigurationKeys.ProducerConfigsEnableIdempotence, true),
            Acks = Acks.Leader,

            // Reconnection Settings
            ReconnectBackoffMs =
                configuration.GetValue(ConfigurationKeys.ProducerConfigsReconnectBackoffMs,
                    50), // Faster initial reconnect
            ReconnectBackoffMaxMs =
                configuration.GetValue(ConfigurationKeys.ProducerConfigsReconnectBackoffMaxMs,
                    5000) // Reduced max backoff
        };

        var securityProtocolString = configuration[ConfigurationKeys.KafkaSecurityProtocol] ?? "SASL_SSL";
        _producerConfig.SecurityProtocol = Enum.TryParse<SecurityProtocol>(
            securityProtocolString.Replace("_", string.Empty),
            true,
            out var securityProtocol)
            ? securityProtocol
            : SecurityProtocol.SaslSsl;

        var saslMechanismString = configuration[ConfigurationKeys.KafkaSaslMechanism] ?? "PLAIN";
        _producerConfig.SaslMechanism = Enum.TryParse<SaslMechanism>(saslMechanismString.Replace("_", string.Empty),
            true,
            out var saslMechanism)
            ? saslMechanism
            : SaslMechanism.Plain;

        // Initialize producer pool
        var poolPolicy = new ProducerPoolPolicy(_producerConfig);
        _producerPool = new DefaultObjectPool<IProducer<string, string>>(poolPolicy, 10);
        _producer = CreateProducer();
        _topic = configuration[ConfigurationKeys.KafkaTopic] ?? string.Empty;

        // Start metrics reporting timer (every 10 seconds)
        _metricsReportingTimer = new Timer(ReportMetrics, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<bool> IsHealthy(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_circuitBreaker.IsOpen)
                return false;

            var heartbeat = new Message<string, string>
            {
                Key = "health_check",
                Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
            };

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);
            var result = await _producer.ProduceAsync(_topic, heartbeat, linkedCts.Token);
            return result.Status == PersistenceStatus.Persisted;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Health check failed");
            return false;
        }
    }

    public async Task ProduceMessageAsync(Message<string, string> message,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (string.IsNullOrWhiteSpace(_topic)) return;
        if (_circuitBreaker.IsOpen) throw new InvalidOperationException("Circuit breaker is open");

        await _throttle.WaitAsync(TimeSpan.FromSeconds(3), cancellationToken);
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var messageValue = JsonConvert.SerializeObject(message);

            var producer = _producerPool.Get();
            try
            {
                var deliveryResult = await producer.ProduceAsync(_topic, message, cancellationToken);

                stopwatch.Stop();
                _metrics.RecordLatency(stopwatch.Elapsed.TotalMilliseconds);
                _metrics.RecordMessage(_topic, messageValue.Length);

                if (deliveryResult.Status != PersistenceStatus.Persisted)
                {
                    _logger.LogWarning("Message not persisted. Status: {Status}", deliveryResult.Status);
                    _circuitBreaker.RecordFailure();
                }
                else
                {
                    _circuitBreaker.RecordSuccess();
                }
            }
            finally
            {
                _producerPool.Return(producer);
            }
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to deliver message: {Reason}", ex.Error.Reason);
            _circuitBreaker.RecordFailure();

            if (ex.Error.Code == ErrorCode.Local_Transport)
            {
                await HandleErrorAsync(ex.Error);
                await ProduceMessageAsync(message, cancellationToken);
            }
            else
            {
                throw;
            }
        }
        finally
        {
            _throttle.Release();
        }
    }

    public async Task ProduceBatchAsync(Message<string, string>?[] messages,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (string.IsNullOrWhiteSpace(_topic)) return;
        if (_circuitBreaker.IsOpen) throw new InvalidOperationException("Circuit breaker is open");

        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task<DeliveryResult<string, string>>>(MaxBatchSizeMessages);
        var batchSize = 0;

        using var memoryOwner = MemoryPool<Task<DeliveryResult<string, string>>>.Shared.Rent(MaxBatchSizeMessages);

        var producer = _producerPool.Get();
        try
        {
            foreach (var message in messages)
                if (message != null)
                {
                    batchSize += message.Value.Length;
                    tasks.Add(producer.ProduceAsync(_topic, message, cancellationToken));

                    if (tasks.Count >= MaxBatchSizeMessages || batchSize >= MaxBatchSizeBytes ||
                        message.Equals(messages[^1]))
                    {
                        await ProcessBatchAsync(tasks, stopwatch, producer);
                        tasks.Clear();
                        batchSize = 0;
                        stopwatch.Restart();
                    }
                }
        }
        finally
        {
            _producerPool.Return(producer);
        }
    }

    // New method to produce to a specific topic
    public async Task ProduceBatchToTopicAsync(Message<string, string>?[] messages, string topic,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (string.IsNullOrWhiteSpace(topic)) return;
        if (_circuitBreaker.IsOpen) throw new InvalidOperationException("Circuit breaker is open");

        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task<DeliveryResult<string, string>>>(MaxBatchSizeMessages);
        var batchSize = 0;

        using var memoryOwner = MemoryPool<Task<DeliveryResult<string, string>>>.Shared.Rent(MaxBatchSizeMessages);

        var producer = _producerPool.Get();
        try
        {
            foreach (var message in messages)
                if (message != null)
                {
                    batchSize += message.Value.Length;
                    tasks.Add(producer.ProduceAsync(topic, message, cancellationToken));

                    if (tasks.Count >= MaxBatchSizeMessages || batchSize >= MaxBatchSizeBytes ||
                        message.Equals(messages[^1]))
                    {
                        await ProcessBatchWithTopicAsync(tasks, stopwatch, producer, topic);
                        tasks.Clear();
                        batchSize = 0;
                        stopwatch.Restart();
                    }
                }
        }
        finally
        {
            _producerPool.Return(producer);
        }
    }

    public ProducerMetricsSnapshot GetMetrics()
    {
        ThrowIfDisposed();
        return _metrics.GetSnapshot();
    }

    public void ResetMetrics()
    {
        ThrowIfDisposed();
        _metrics.Reset();
    }

    ~KafkaProducerOptimizedService()
    {
        Dispose(false);
    }

    private IProducer<string, string> CreateProducer()
    {
        return new ProducerBuilder<string, string>(_producerConfig)
            .SetErrorHandler((_, error) => HandleKafkaError(error))
            .Build();
    }

    private void HandleKafkaError(Error error)
    {
        _logger.LogError("Kafka Error: {Reason}. Code: {Code}", error.Reason, error.Code);

        if (error.Code == ErrorCode.Local_Transport)
            Task.Run(async () => await HandleErrorAsync(error))
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        _logger.LogError(t.Exception, "Error handling Kafka error");
                });
    }

    private async Task ProcessBatchAsync(
        IReadOnlyCollection<Task<DeliveryResult<string, string>>> tasks,
        Stopwatch stopwatch,
        IProducer<string, string> producer)
    {
        try
        {
            var results = await Task.WhenAll(tasks);
            stopwatch.Stop();

            var avgLatency = stopwatch.Elapsed.TotalMilliseconds / tasks.Count;
            _metrics.RecordLatency(avgLatency);

            var failureCount = 0;
            foreach (var result in results)
            {
                _metrics.RecordMessage(_topic, result.Message.Value.Length);
                if (result.Status != PersistenceStatus.Persisted)
                    failureCount++;
            }

            if (failureCount > 0)
            {
                _circuitBreaker.RecordFailure();
                _logger.LogWarning("{Count} messages in batch not persisted", failureCount);
            }
            else
            {
                _circuitBreaker.RecordSuccess();
            }

            producer.Flush(TimeSpan.FromSeconds(5));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing batch");
            _circuitBreaker.RecordFailure();
            throw new InvalidOperationException("Error processing batch");
        }
    }

    // New method to process batch with a specific topic
    private async Task ProcessBatchWithTopicAsync(
        IReadOnlyCollection<Task<DeliveryResult<string, string>>> tasks,
        Stopwatch stopwatch,
        IProducer<string, string> producer,
        string topic)
    {
        try
        {
            var results = await Task.WhenAll(tasks);
            stopwatch.Stop();

            var avgLatency = stopwatch.Elapsed.TotalMilliseconds / tasks.Count;
            _metrics.RecordLatency(avgLatency);

            var failureCount = 0;
            foreach (var result in results)
            {
                _metrics.RecordMessage(topic, result.Message.Value.Length);
                if (result.Status != PersistenceStatus.Persisted)
                    failureCount++;
            }

            if (failureCount > 0)
            {
                _circuitBreaker.RecordFailure();
                _logger.LogWarning("{Count} messages in batch not persisted for topic {Topic}", failureCount, topic);
            }
            else
            {
                _circuitBreaker.RecordSuccess();
            }

            producer.Flush(TimeSpan.FromSeconds(5));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing batch for topic {Topic}", topic);
            _circuitBreaker.RecordFailure();
            throw new InvalidOperationException($"Error processing batch for topic {topic}");
        }
    }

    private async Task HandleErrorAsync(Error error)
    {
        if (error.Code == ErrorCode.Local_Transport)
        {
            var backoffMs = 50;
            var retryCount = 0;

            while (retryCount < MaxRetryCount)
                try
                {
                    await Task.Delay(backoffMs);
                    var heartbeat = new Message<string, string>
                    {
                        Key = "heartbeat",
                        Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
                    };

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    var result = await _producer.ProduceAsync(_topic, heartbeat, cts.Token);

                    if (result.Status == PersistenceStatus.Persisted)
                    {
                        _logger.LogInformation("Reconnected to Kafka successfully");
                        _circuitBreaker.Reset();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Reconnection attempt {Count} failed: {Message}", retryCount + 1,
                        ex.Message);
                    backoffMs = Math.Min(backoffMs * 2, 5000);
                    retryCount++;
                }

            _logger.LogError("Failed to reconnect to Kafka after {MaxRetryCount} attempts", MaxRetryCount);
        }
    }

    private void ThrowIfDisposed()
    {
        if (!_disposed) return;
        throw new ObjectDisposedException(nameof(KafkaProducerOptimizedService));
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
            try
            {
                _metricsReportingTimer.Dispose();
                _throttle.Dispose();
                _producer.Flush(TimeSpan.FromSeconds(5));
                _producer.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during producer disposal: {Message}", ex.Message);
            }

        _disposed = true;
    }

    private void ReportMetrics(object? state)
    {
        if (_disposed) return;

        var snapshot = _metrics.GetSnapshot();

        _logger.LogInformation(
            "Producer Metrics: Messages={MessageCount}, " +
            "Throughput={MessagesPerSecond:F2} msg/s, " +
            "Bandwidth={BytesPerSecond:F2} B/s, " +
            "Avg Latency={AverageLatencyMs:F2}ms, " +
            "P95 Latency={P95LatencyMs:F2}ms, " +
            "P99 Latency={P99LatencyMs:F2}ms, " +
            "Circuit Breaker={CircuitState}, " +
            "Uptime={UptimeMs}ms",
            snapshot.MessageCount,
            snapshot.MessagesPerSecond,
            snapshot.BytesPerSecond,
            snapshot.AverageLatencyMs,
            snapshot.P95LatencyMs,
            snapshot.P99LatencyMs,
            _circuitBreaker.IsOpen ? "Open" : "Closed",
            snapshot.UptimeMs);
#if DEBUG
        if (snapshot is { MessageCountByTopic: not null } and { BytesByTopic: not null })
            foreach (var topic in snapshot.MessageCountByTopic.Keys)
                _logger.LogDebug(
                    "Topic {Topic}: Messages={MessageCount}, Bytes={BytesProduced}",
                    topic,
                    snapshot.MessageCountByTopic[topic],
                    snapshot.BytesByTopic[topic]);
#endif
    }

    private static void ValidateConfiguration(IConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration[ConfigurationKeys.KafkaBootstrapServers]))
            throw new InvalidOperationException("Kafka bootstrap servers configuration is missing.");

        if (string.IsNullOrWhiteSpace(configuration[ConfigurationKeys.KafkaTopic]))
            throw new InvalidOperationException("Kafka topic configuration is missing.");

        if (string.IsNullOrWhiteSpace(configuration[ConfigurationKeys.KafkaSaslUsername]) ||
            string.IsNullOrWhiteSpace(configuration[ConfigurationKeys.KafkaSaslPassword]))
            throw new InvalidOperationException("Kafka SASL credentials are missing.");
    }
}

// Helper class for producer pooling
public class ProducerPoolPolicy : IPooledObjectPolicy<IProducer<string, string>>
{
    private readonly ProducerConfig _config;

    /// <summary>
    /// </summary>
    /// <param name="config"></param>
    public ProducerPoolPolicy(ProducerConfig config)
    {
        _config = config;
    }

    public IProducer<string, string> Create()
    {
        return new ProducerBuilder<string, string>(_config)
            .SetErrorHandler((_, error) =>
            {
                // Log errors but don't take action here as the main service handles errors
                Console.Error.WriteLine($"Kafka Error in pooled producer: {error.Reason}. Code: {error.Code}");
            })
            .Build();
    }

    public bool Return(IProducer<string, string> obj)
    {
        try
        {
            // Flush any remaining messages before returning to pool
            obj.Flush(TimeSpan.FromSeconds(2));
            return true;
        }
        catch
        {
            // If flush fails, dispose the producer and create a new one
            try
            {
                obj.Dispose();
            }
            catch
            {
                // Ignore disposal errors
            }

            return false;
        }
    }
}

// Circuit breaker implementation
public class CircuitBreaker
{
    private readonly int _failureThreshold;
    private readonly object _lock = new();
    private readonly TimeSpan _resetTimeout;
    private int _failureCount;
    private bool _isOpen;
    private DateTime? _lastFailureTime;

    /// <summary>
    /// </summary>
    /// <param name="failureThreshold"></param>
    /// <param name="resetTimeout"></param>
    public CircuitBreaker(int failureThreshold, TimeSpan resetTimeout)
    {
        _failureThreshold = failureThreshold;
        _resetTimeout = resetTimeout;
    }

    public bool IsOpen
    {
        get
        {
            lock (_lock)
            {
                if (!_isOpen) return false;

                if (_lastFailureTime.HasValue && DateTime.UtcNow - _lastFailureTime.Value >= _resetTimeout)
                {
                    _isOpen = false;
                    _failureCount = 0;
                    _lastFailureTime = null;
                    return false;
                }

                return true;
            }
        }
    }

    public void RecordFailure()
    {
        lock (_lock)
        {
            _failureCount++;
            _lastFailureTime = DateTime.UtcNow;

            if (_failureCount >= _failureThreshold) _isOpen = true;
        }
    }

    public void RecordSuccess()
    {
        lock (_lock)
        {
            _failureCount = 0;
            if (_isOpen)
            {
                _isOpen = false;
                _lastFailureTime = null;
            }
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            _failureCount = 0;
            _isOpen = false;
            _lastFailureTime = null;
        }
    }
}