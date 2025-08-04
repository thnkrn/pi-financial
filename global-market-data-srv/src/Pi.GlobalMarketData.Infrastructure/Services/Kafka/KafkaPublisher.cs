using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;

namespace Pi.GlobalMarketData.Infrastructure.Services.Kafka;

public sealed class KafkaPublisher<TKey, TValue> : IKafkaPublisher<TKey, TValue>, IDisposable
{
    private readonly IProducer<TKey, TValue> _producer;
    private bool _disposed;

    public KafkaPublisher(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration[ConfigurationKeys.KafkaBootstrapServers],
            SaslUsername = configuration[ConfigurationKeys.KafkaSaslUsername],
            SaslPassword = configuration[ConfigurationKeys.KafkaSaslPassword],
        };

        var securityProtocolString = configuration[ConfigurationKeys.KafkaSecurityProtocol] ?? "SASL_SSL";
        config.SecurityProtocol = Enum.TryParse<SecurityProtocol>(securityProtocolString.Replace("_", string.Empty), true,
            out var securityProtocol) ? securityProtocol : SecurityProtocol.SaslSsl;

        var saslMechanismString = configuration[ConfigurationKeys.KafkaSaslMechanism] ?? "PLAIN";
        config.SaslMechanism = Enum.TryParse<SaslMechanism>(saslMechanismString.Replace("_", string.Empty), true,
            out var saslMechanism) ? saslMechanism : SaslMechanism.Plain;

        _producer = new ProducerBuilder<TKey, TValue>(config).Build();
    }

    public async Task<DeliveryResult<TKey, TValue>> PublishAsync(string topic, Message<TKey, TValue> message)
    {
        var result = await _producer.ProduceAsync(topic, message);
        return result;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _producer.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~KafkaPublisher()
    {
        Dispose(false);
    }
}