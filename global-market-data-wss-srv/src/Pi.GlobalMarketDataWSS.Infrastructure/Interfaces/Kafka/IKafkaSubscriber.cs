namespace Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Kafka;

public interface IKafkaSubscriber
{
    Task SubscribeAsync(CancellationToken cancellationToken);
    Task UnsubscribeAsync();
}