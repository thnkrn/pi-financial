using System.Text.Encodings.Web;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Pi.SetMarketDataWSS.DataStreamer;

public sealed class KafkaPublisher : IDisposable
{
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<KafkaPublisher> _logger;
    private readonly IProducer<string, string> _producer;
    private bool _disposed;

    public KafkaPublisher(ILogger<KafkaPublisher> logger)
    {
        _logger = logger;

        var ref1 = ConfigurationHelper.GetConfiguration().GetValue<string>("KAFKA:SASL_REF_1");
        var ref2 = ConfigurationHelper.GetConfiguration().GetValue<string>("KAFKA:SASL_REF_2");
        var config = new ProducerConfig
        {
            BootstrapServers = "pkc-312o0.ap-southeast-1.aws.confluent.cloud:9092",
            SaslUsername = ref1,
            SaslPassword = ref2,

            // Ensure messages are not lost
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 3
        };

        var securityProtocolString = "SASL_SSL";
        config.SecurityProtocol = Enum.TryParse<SecurityProtocol>(securityProtocolString.Replace("_", string.Empty),
            true,
            out var securityProtocol)
            ? securityProtocol
            : SecurityProtocol.SaslSsl;

        var saslMechanismString = "PLAIN";
        config.SaslMechanism = Enum.TryParse<SaslMechanism>(saslMechanismString.Replace("_", string.Empty), true,
            out var saslMechanism)
            ? saslMechanism
            : SaslMechanism.Plain;

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Error: {Reason}", e.Reason))
            .SetLogHandler((_, m) => _logger.LogDebug("Kafka: {Message}", m.Message))
            .Build();

        _jsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
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
            Value = JsonSerializer.Serialize(message, _jsonOptions)
        };

        try
        {
            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);
            _logger.LogDebug("Delivered '{Value}' to '{TopicPartitionOffset}'",
                deliveryResult.Value, deliveryResult.TopicPartitionOffset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Delivery failed: {Reason}", ex.Error.Reason);
            throw new InvalidOperationException(ex.Message, ex);
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
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
        }

        _disposed = true;
    }

    ~KafkaPublisher()
    {
        Dispose(false);
    }
}