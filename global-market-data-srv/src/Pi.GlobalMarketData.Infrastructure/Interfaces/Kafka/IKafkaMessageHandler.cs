using Confluent.Kafka;

namespace Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;
public interface IKafkaMessageHandler<TKey, TValue>
{
    Task HandleAsync(ConsumeResult<TKey, TValue> consumeResult);
}

public interface IKafkaMessageV2Handler<in T>
{
    Task<bool> HandleAsync(T message);
}