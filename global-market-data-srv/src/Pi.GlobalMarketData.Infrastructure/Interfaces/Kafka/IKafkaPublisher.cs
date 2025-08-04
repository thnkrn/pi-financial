using Confluent.Kafka;

namespace Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;

public interface IKafkaPublisher<TKey, TValue>
{
    Task<DeliveryResult<TKey, TValue>> PublishAsync(string topic, Message<TKey, TValue> message);
}