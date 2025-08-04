using System.Diagnostics;
using System.Globalization;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

public interface IKafkaProducerService
{
    Task ProduceMessageAsync(Message<string, string> message, CancellationToken cancellationToken = default);
    Task ProduceBatchAsync(Message<string, string>[] messages, CancellationToken cancellationToken = default);
    ProducerMetricsSnapshot GetMetrics();
    void ResetMetrics();
}

public sealed class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly ProducerMetrics _metrics;
    private readonly Timer _metricsReportingTimer;
    private readonly IProducer<string, string> _producer;

    private readonly SemaphoreSlim _throttle;
    private readonly string _topic;
    private bool _disposed;


    public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
    {
        _logger = logger;
        _metrics = new ProducerMetrics();
        _throttle = new SemaphoreSlim(2000); // Max concurrent requests

        // Kafka configurations
        var bootstrapServers = configuration[ConfigurationKeys.KafkaBootstrapServers];

        if (string.IsNullOrWhiteSpace(bootstrapServers))
            throw new InvalidOperationException("Kafka bootstrap servers configuration is missing.");

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            SaslUsername = configuration[ConfigurationKeys.KafkaSaslUsername],
            SaslPassword = configuration[ConfigurationKeys.KafkaSaslPassword],
            // Performance Optimizations
            BatchSize = 2097152,
            LingerMs = 20,
            CompressionType = CompressionType.Snappy,
            QueueBufferingMaxMessages = 2000000,
            QueueBufferingMaxKbytes = 4194304,
            MessageSendMaxRetries = 5,
            RetryBackoffMs = 200,
            // Connection Optimizations
            SocketTimeoutMs = 30000,
            MessageTimeoutMs = 30000,
            RequestTimeoutMs = 30000,
            MaxInFlight = 5,
            EnableIdempotence = true,
            Acks = Acks.All,
            // Reconnection Settings
            ReconnectBackoffMs = 100,
            ReconnectBackoffMaxMs = 10000
        };

        var securityProtocolString = configuration[ConfigurationKeys.KafkaSecurityProtocol] ?? "SASL_SSL";
        config.SecurityProtocol = Enum.TryParse<SecurityProtocol>(securityProtocolString.Replace("_", string.Empty),
            true,
            out var securityProtocol)
            ? securityProtocol
            : SecurityProtocol.SaslSsl;

        var saslMechanismString = configuration[ConfigurationKeys.KafkaSaslMechanism] ?? "PLAIN";
        config.SaslMechanism = Enum.TryParse<SaslMechanism>(saslMechanismString.Replace("_", string.Empty), true,
            out var saslMechanism)
            ? saslMechanism
            : SaslMechanism.Plain;

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, ex) =>
            {
                _logger.LogError("Kafka Error: {Reason}. Code: {Code}", ex.Reason, ex.Code);
                HandleError(ex);
            })
            .Build();

        _topic = configuration[ConfigurationKeys.KafkaTopic] ?? string.Empty;

        // Start metrics reporting timer (every 5 seconds)
        _metricsReportingTimer = new Timer(ReportMetrics, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task ProduceMessageAsync(Message<string, string> message,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (string.IsNullOrWhiteSpace(_topic)) return;

        await _throttle.WaitAsync(cancellationToken);
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var messageValue = JsonConvert.SerializeObject(message);
            var deliveryResult = await _producer.ProduceAsync(_topic, message, cancellationToken);

            stopwatch.Stop();
            _metrics.RecordLatency(stopwatch.Elapsed.TotalMilliseconds);
            _metrics.RecordMessage(_topic, messageValue.Length);

            if (deliveryResult.Status != PersistenceStatus.Persisted)
                _logger.LogWarning("Message not persisted. Status: {Status}", deliveryResult.Status);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to deliver message: {Reason}", ex.Error.Reason);
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

    public async Task ProduceBatchAsync(Message<string, string>[] messages,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (string.IsNullOrWhiteSpace(_topic)) return;

        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task<DeliveryResult<string, string>>>();
        var batchSize = 0;

        foreach (var message in messages)
        {
            batchSize += message.Value.Length;

            tasks.Add(_producer.ProduceAsync(_topic, message, cancellationToken));

            // Process batch when we hit 1000 messages or 1MB total size or last message
            if (tasks.Count >= 1000 || batchSize >= 1_000_000 || message.Equals(messages[^1]))
            {
                // Wait for all messages in current batch
                var results = await Task.WhenAll(tasks);

                // Record metrics once for the batch
                stopwatch.Stop();
                var avgLatency = stopwatch.Elapsed.TotalMilliseconds / tasks.Count;
                _metrics.RecordLatency(avgLatency);

                // Record size for each message in the batch
                foreach (var result in results) _metrics.RecordMessage(_topic, result.Message.Value.Length);

                // Clear for next batch
                tasks.Clear();
                batchSize = 0;

                // Ensure messages are flushed
                _producer.Flush(TimeSpan.FromSeconds(10));

                // Reset stopwatch for next batch
                stopwatch.Restart();
            }
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

    private async Task HandleErrorAsync(Error error)
    {
        if (error.Code == ErrorCode.Local_Transport)
        {
            var backoffMs = 100;
            var retryCount = 0;
            const int maxRetries = 5;

            while (retryCount < maxRetries)
                try
                {
                    await Task.Delay(backoffMs);
                    var heartbeat = new Message<string, string>
                    {
                        Key = "heartbeat",
                        Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
                    };

                    var result = await _producer.ProduceAsync(_topic, heartbeat);
                    if (result.Status == PersistenceStatus.Persisted)
                    {
                        _logger.LogInformation("Reconnected to Kafka successfully");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Reconnection attempt {Count} failed: {Message}",
                        retryCount + 1, ex.Message);
                    backoffMs = Math.Min(backoffMs * 2, 10000);
                    retryCount++;
                }
        }
    }

    private void ThrowIfDisposed()
    {
        if (!_disposed) return;
        throw new ObjectDisposedException(nameof(KafkaProducerService));
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
            try
            {
                _metricsReportingTimer.Dispose();
                try
                {
                    _producer.Flush(TimeSpan.FromSeconds(10));
                }
                catch (KafkaException ex)
                {
                    _logger.LogError(ex, "Error during flush: {Message}", ex.Message);
                }
                finally
                {
                    _producer.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during producer disposal: {Message}", ex.Message);
            }

        _disposed = true;
    }

    ~KafkaProducerService()
    {
        Dispose(false);
    }

    private void ReportMetrics(object? state)
    {
        if (state == null) return;

        var snapshot = _metrics.GetSnapshot();

        _logger.LogInformation(
            "Producer Metrics: Messages={MessageCount}, " +
            "Throughput={MessagesPerSecond:F2} msg/s, " +
            "Bandwidth={BytesPerSecond:F2} B/s, " +
            "Avg Latency={AverageLatencyMs:F2}ms, " +
            "P95 Latency={P95LatencyMs:F2}ms, " +
            "P99 Latency={P99LatencyMs:F2}ms, " +
            "Uptime={UptimeMs}ms",
            snapshot.MessageCount,
            snapshot.MessagesPerSecond,
            snapshot.BytesPerSecond,
            snapshot.AverageLatencyMs,
            snapshot.P95LatencyMs,
            snapshot.P99LatencyMs,
            snapshot.UptimeMs);

        if (snapshot is { MessageCountByTopic: not null } and { BytesByTopic: not null })
            // Log per-topic metrics
            foreach (var topic in snapshot.MessageCountByTopic.Keys)
                _logger.LogInformation(
                    "Topic {Topic}: Messages={MessageCount}, Bytes={BytesProduced}",
                    topic,
                    snapshot.MessageCountByTopic[topic],
                    snapshot.BytesByTopic[topic]);
    }

    private void HandleError(Error error)
    {
        if (error.Code == ErrorCode.Local_Transport)
        {
            var backoffMs = 100;
            while (true)
                try
                {
                    Thread.Sleep(backoffMs);
                    var result = _producer.ProduceAsync(_topic,
                            new Message<string, string>
                                { Key = "heartbeat", Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) })
                        .GetAwaiter().GetResult();

                    if (result.Status == PersistenceStatus.Persisted)
                    {
                        _logger.LogInformation("Reconnected to Kafka successfully");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Reconnection attempt failed: {Message}", ex.Message);
                    backoffMs = Math.Min(backoffMs * 2, 10000);
                }
        }
    }
}