namespace Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Kafka;

public interface IKafkaMessageHandler<in T>
{
    Task HandleAsync(T message);
}