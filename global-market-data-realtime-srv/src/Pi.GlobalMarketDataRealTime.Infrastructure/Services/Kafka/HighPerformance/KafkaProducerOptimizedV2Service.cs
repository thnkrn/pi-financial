using System.Diagnostics;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

public interface IKafkaProducerOptimizedV2Service
{
    Task ProduceAsync(string topic, Message<string, string> message, CancellationToken cancellationToken = default);

    Task ProduceBatchAsync(string topic, IEnumerable<Message<string, string>> messages,
        CancellationToken cancellationToken = default);

    Task<bool> IsHealthy(CancellationToken cancellationToken = default);
}

public sealed class KafkaProducerOptimizedV2Service : IKafkaProducerOptimizedV2Service, IDisposable
{
    private readonly string _defaultTopic;
    private readonly TimeSpan _healthCheckInterval = TimeSpan.FromMinutes(2);
    private readonly SemaphoreSlim _healthCheckSemaphore = new(1, 1);
    private readonly Timer _healthCheckTimer;
    private readonly ILogger<KafkaProducerOptimizedV2Service> _logger;
    private readonly IProducer<string, string> _producer;
    private readonly object _reconnectLock = new();
    private int _disconnectCount;
    private bool _disposed;
    private DateTime _lastReconnectAttempt = DateTime.MinValue;

    public KafkaProducerOptimizedV2Service(IConfiguration configuration,
        ILogger<KafkaProducerOptimizedV2Service> logger)
    {
        _logger = logger;

        // Validate configuration before proceeding
        ValidateConfiguration(configuration);

        // Kafka configurations
        var bootstrapServers = configuration[ConfigurationKeys.KafkaBootstrapServers];
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            SaslUsername = configuration[ConfigurationKeys.KafkaSaslUsername],
            SaslPassword = configuration[ConfigurationKeys.KafkaSaslPassword],

            // Performance Optimizations
            BatchSize = configuration.GetValue(ConfigurationKeys.ProducerConfigsBatchSize, 4194304), // 4MB batch size
            LingerMs = configuration.GetValue(ConfigurationKeys.ProducerConfigsLingerMs, 10), // 10ms linger
            CompressionType = CompressionType.Lz4,

            // Message settings
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 500,
            MessageTimeoutMs = 10000,

            // Socket configurations
            SocketTimeoutMs = configuration.GetValue(ConfigurationKeys.ProducerConfigsSocketTimeoutMs, 30000),
            SocketSendBufferBytes =
                configuration.GetValue(ConfigurationKeys.ProducerConfigsSocketSendBufferBytes, 1048576), // 1MB

            // Performance tuning
            QueueBufferingMaxMessages =
                configuration.GetValue(ConfigurationKeys.producerConfigsQueueBufferingMaxMessages, 100000),
            QueueBufferingMaxKbytes =
                configuration.GetValue(ConfigurationKeys.producerConfigsQueueBufferingMaxKBytes, 4194304), // 4MB

            // Connection Settings
            SocketConnectionSetupTimeoutMs = 60000,
            SocketMaxFails = 3,
            ReconnectBackoffMs = 1000,
            ReconnectBackoffMaxMs = 10000,

            // Acks setting
            Acks = Acks.Leader
        };

        var securityProtocolString = configuration[ConfigurationKeys.KafkaSecurityProtocol] ?? "SASL_SSL";
        config.SecurityProtocol = Enum.TryParse<SecurityProtocol>(
            securityProtocolString.Replace("_", string.Empty),
            true,
            out var securityProtocol)
            ? securityProtocol
            : SecurityProtocol.SaslSsl;

        var saslMechanismString = configuration[ConfigurationKeys.KafkaSaslMechanism] ?? "PLAIN";
        config.SaslMechanism = Enum.TryParse<SaslMechanism>(saslMechanismString.Replace("_", string.Empty),
            true,
            out var saslMechanism)
            ? saslMechanism
            : SaslMechanism.Plain;

        // Get Topic
        _defaultTopic = configuration[ConfigurationKeys.KafkaTopic] ?? string.Empty;

        // Create the producer
        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, error) =>
            {
                _logger.LogError("Kafka Error: {Reason}. Code: {Code}", error.Reason, error.Code);
                if (error.Code == ErrorCode.Local_Transport)
                {
                    Interlocked.Increment(ref _disconnectCount);
                    _logger.LogWarning("Kafka disconnection detected. Total disconnects: {Count}", _disconnectCount);
                }
            })
            .Build();

        _healthCheckTimer = new Timer(HealthCheckCallback, null,
            _healthCheckInterval,
            _healthCheckInterval
        );

        _logger.LogInformation("Kafka health check timer initialized with interval of {Interval} minutes",
            _healthCheckInterval.TotalMinutes);
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
            EnsureProducerConnected();

            var heartbeat = new Message<string, string>
            {
                Key = "health_check",
                Value = DateTime.UtcNow.ToString("o")
            };

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);
            var result = await _producer.ProduceAsync(_defaultTopic, heartbeat, linkedCts.Token);
            return result.Status == PersistenceStatus.Persisted;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Health check failed");
            return false;
        }
    }

    public async Task ProduceAsync(string topic, Message<string, string> message,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(topic))
        {
            topic = _defaultTopic;
            if (string.IsNullOrWhiteSpace(topic))
            {
                _logger.LogWarning("No topic specified and no default topic configured");
                return;
            }
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await _producer.ProduceAsync(topic, message, cancellationToken);
            stopwatch.Stop();

            if (result.Status != PersistenceStatus.Persisted)
                _logger.LogWarning("Message not persisted to topic {Topic}. Status: {Status}", topic, result.Status);
            else if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Message sent to {Topic} in {ElapsedMs}ms", topic, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to produce message to topic {Topic}", topic);
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    public async Task ProduceBatchAsync(string topic, IEnumerable<Message<string, string>> messages,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(topic))
        {
            topic = _defaultTopic;
            if (string.IsNullOrWhiteSpace(topic))
            {
                _logger.LogWarning("No topic specified and no default topic configured");
                return;
            }
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var messageList = messages.ToList();

            if (messageList.Count == 0)
            {
                _logger.LogDebug("No messages to send in batch");
                return;
            }

            var deliveryReportTasks = new List<Task<DeliveryResult<string, string>>>(messageList.Count);

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var message in messageList)
                deliveryReportTasks.Add(_producer.ProduceAsync(topic, message, cancellationToken));

            // After all messages are queued, flush to ensure they're sent
            _producer.Flush(TimeSpan.FromSeconds(5));

            // Now wait for all delivery reports to complete
            await Task.WhenAll(deliveryReportTasks);

            stopwatch.Stop();

            _logger.LogDebug("Batch of {Count} messages sent to topic {Topic} in {ElapsedMs}ms",
                messageList.Count, topic, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to produce batch to topic {Topic}", topic);
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    private async void HealthCheckCallback(object? state)
    {
        try
        {
            if (await _healthCheckSemaphore.WaitAsync(0))
                try
                {
                    _logger.LogDebug("Performing periodic Kafka connection health check");

                    var isHealthy = await IsHealthy(CancellationToken.None);
                    if (isHealthy)
                        _logger.LogDebug("Kafka connection health check passed");
                    else
                        _logger.LogWarning(
                            "Kafka connection health check failed; The connection should auto-reconnect");
                }
                finally
                {
                    _healthCheckSemaphore.Release();
                }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Kafka health check");
        }
    }

    private void EnsureProducerConnected()
    {
        if ((DateTime.UtcNow - _lastReconnectAttempt).TotalMinutes < 2)
            return;

        lock (_reconnectLock)
        {
            try
            {
                _logger.LogDebug("Ensuring Kafka producer connection is active");
                _producer.Flush(TimeSpan.FromSeconds(5));
                _lastReconnectAttempt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during producer flush attempt");
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (!_disposed) return;
        throw new ObjectDisposedException(nameof(KafkaProducerOptimizedV2Service));
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
            try
            {
                // Dispose timer
                _healthCheckTimer.Dispose();
                _producer.Flush(TimeSpan.FromSeconds(5));
                _producer.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during producer disposal: {Message}", ex.Message);
            }

        _disposed = true;
    }

    ~KafkaProducerOptimizedV2Service()
    {
        Dispose(false);
    }

    private static void ValidateConfiguration(IConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration[ConfigurationKeys.KafkaBootstrapServers]))
            throw new InvalidOperationException("Kafka bootstrap servers configuration is missing.");

        if (string.IsNullOrWhiteSpace(configuration[ConfigurationKeys.KafkaSaslUsername])
            || string.IsNullOrWhiteSpace(configuration[ConfigurationKeys.KafkaSaslPassword]))
            throw new InvalidOperationException("Kafka SASL credentials are missing.");
    }
}