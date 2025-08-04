namespace Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Kafka;

public interface IKafkaBatchSubscriber
{
    Task SubscribeAsync(CancellationToken cancellationToken);
    Task UnsubscribeAsync();
}

public interface IKafkaBatchV2Subscriber
{
    Task SubscribeAsync(CancellationToken cancellationToken);
    Task UnsubscribeAsync();
}