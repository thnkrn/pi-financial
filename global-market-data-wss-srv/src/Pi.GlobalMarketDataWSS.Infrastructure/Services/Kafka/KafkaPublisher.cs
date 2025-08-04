using System.Text.Encodings.Web;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Kafka;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Services.Kafka;

public sealed class KafkaPublisher : IKafkaPublisher, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private bool _disposed;

    public KafkaPublisher(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
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

        var kafkaMessage = new Message<Null, string>
        {
            Value = JsonSerializer.Serialize(message, options)
        };

        await _producer.ProduceAsync(topic, kafkaMessage);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing) _producer.Dispose();

        _disposed = true;
    }

    ~KafkaPublisher()
    {
        Dispose(false);
    }
}