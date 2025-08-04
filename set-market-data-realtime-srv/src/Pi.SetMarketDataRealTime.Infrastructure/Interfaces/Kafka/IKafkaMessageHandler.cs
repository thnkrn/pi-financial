namespace Pi.SetMarketDataRealTime.Infrastructure.Interfaces.Kafka;

public interface IKafkaMessageHandler<in T>
{
    Task HandleAsync(T message);
}