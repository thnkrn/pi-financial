using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Pi.SetMarketDataRealTime.Domain.ConstantConfigurations;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.Kafka;

public static class ProducerConfigHelper
{
    public static ProducerConfig ProducerConfiguration(IConfiguration configuration)
    {
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

        return config;
    }
}