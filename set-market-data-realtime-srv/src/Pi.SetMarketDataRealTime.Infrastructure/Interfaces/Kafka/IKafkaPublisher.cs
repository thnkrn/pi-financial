namespace Pi.SetMarketDataRealTime.Infrastructure.Interfaces.Kafka;

public interface IKafkaPublisher
{
    Task PublishAsync<T>(string topic, T message, string? key = null);
    Task PublishBatchAsync<T>(string topic, IEnumerable<T> messages);
}