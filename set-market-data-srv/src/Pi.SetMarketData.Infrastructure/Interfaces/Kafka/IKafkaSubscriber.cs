using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;

namespace Pi.SetMarketData.Infrastructure.Interfaces.Kafka;

public interface IKafkaSubscriber<TKey, TValue>
{
    Task SubscribeAsync(CancellationToken cancellationToken);
    Task UnsubscribeAsync();
}

public interface IKafkaV2Subscriber
{
    Task SubscribeAsync(CancellationToken cancellationToken);
    Task UnsubscribeAsync();
}