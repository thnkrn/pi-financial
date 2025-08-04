using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Infrastructure.Exceptions;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Kafka;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka;

public sealed class KafkaPublisher : IKafkaPublisher, IDisposable
{
    private readonly ILogger<KafkaPublisher> _logger;
    private readonly IProducer<string, string> _producer;
    private bool _disposed;

    public KafkaPublisher(IConfiguration configuration, ILogger<KafkaPublisher> logger)
    {
        _logger = logger;

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
            Acks = Acks.All,

            // Reconnection Settings
            ReconnectBackoffMs =
                configuration.GetValue(ConfigurationKeys.ProducerConfigsReconnectBackoffMs,
                    50), // Faster initial reconnect
            ReconnectBackoffMaxMs =
                configuration.GetValue(ConfigurationKeys.ProducerConfigsReconnectBackoffMaxMs,
                    5000) // Reduced max backoff
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
            .SetErrorHandler((_, e) => _logger.LogError("Error: {Reason}", e.Reason))
            .SetLogHandler((_, m) => _logger.LogDebug("Kafka: {Message}", m.Message))
            .Build();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task PublishAsync<T>(string topic, T message)
    {
        var kafkaMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(), // Ensure unique key for each message
            Value = JsonConvert.SerializeObject(message)
        };

        try
        {
            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);
            var logMsg = $"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'";
            _logger.LogDebug("{Log}", logMsg);
        }
        catch (ProduceException<string, string> e)
        {
            var logMsg = $"Delivery failed: {e.Error.Reason}";
            _logger.LogError(e, "{Log}", logMsg);
            throw new InfrastructureServiceException(logMsg, e);
        }
    }

    public async Task PublishBatchAsync<T>(string topic, IEnumerable<T> messages)
    {
        var tasks = messages.Select(message => PublishAsync(topic, message));
        await Task.WhenAll(tasks);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
            try
            {
                _producer.Flush(TimeSpan.FromSeconds(10));
                _producer.Dispose();
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Exception occurred while disposing Kafka producer.");
            }

        _disposed = true;
    }

    ~KafkaPublisher()
    {
        Dispose(false);
    }
}