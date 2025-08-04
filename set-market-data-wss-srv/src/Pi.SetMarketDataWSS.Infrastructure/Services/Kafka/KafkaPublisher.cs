using System.Text.Encodings.Web;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Pi.SetMarketDataWSS.Domain.ConstantConfigurations;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Kafka;

namespace Pi.SetMarketDataWSS.Infrastructure.Services.Kafka;

public sealed class KafkaPublisher : IKafkaPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private bool _disposed;

    public KafkaPublisher(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration[ConfigurationKeys.KafkaBootstrapServers],
            SaslUsername = configuration[ConfigurationKeys.KafkaSaslUsername],
            SaslPassword = configuration[ConfigurationKeys.KafkaSaslPassword]
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

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task PublishAsync<T>(string topic, T message)
    {
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var kafkaMessage = new Message<string, string>
        {
            Value = JsonSerializer.Serialize(message, options)
        };

        await _producer.ProduceAsync(topic, kafkaMessage);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) _producer.Dispose();

        _disposed = true;
    }

    ~KafkaPublisher()
    {
        Dispose(false);
    }
}