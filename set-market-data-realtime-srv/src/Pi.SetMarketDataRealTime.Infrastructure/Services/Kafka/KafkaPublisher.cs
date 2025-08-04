using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.Kafka;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.Kafka;

public sealed class KafkaPublisher : IKafkaPublisher, IDisposable
{
    private readonly ILogger<KafkaPublisher> _logger;
    private readonly IProducer<string, string> _producer;
    private bool _disposed;

    public KafkaPublisher(IConfiguration configuration, ILogger<KafkaPublisher> logger)
    {
        var config = ProducerConfigHelper.ProducerConfiguration(configuration);

        _logger = logger;
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

    public async Task PublishAsync<T>(string topic, T message, string? key = null)
    {
        if (string.IsNullOrEmpty(key))
            key = Guid.NewGuid().ToString();

        var timestamp = DateTime.UtcNow;
        var kafkaMessage = new Message<string, string>
        {
            Key = key,
            Value = JsonConvert.SerializeObject(message),
            Timestamp = new Timestamp(timestamp, TimestampType.CreateTime),
            Headers = new Headers
            {
                { "timestamp", BitConverter.GetBytes(timestamp.Ticks) }
            }
        };

        try
        {
            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);
            _logger.LogDebug("Delivered '{DeliveryResultValue}' to '{DeliveryResultTopicPartitionOffset}'",
                deliveryResult.Value, deliveryResult.TopicPartitionOffset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Delivery failed: {ErrorReason}", ex.Error.Reason);

            //Rethrow exception
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