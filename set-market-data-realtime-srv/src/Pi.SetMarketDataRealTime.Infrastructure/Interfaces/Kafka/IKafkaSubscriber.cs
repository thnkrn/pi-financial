namespace Pi.SetMarketDataRealTime.Infrastructure.Interfaces.Kafka;

public interface IKafkaSubscriber<TKey, TValue>
{
    Task SubscribeAsync(CancellationToken cancellationToken);
    Task UnsubscribeAsync();
}