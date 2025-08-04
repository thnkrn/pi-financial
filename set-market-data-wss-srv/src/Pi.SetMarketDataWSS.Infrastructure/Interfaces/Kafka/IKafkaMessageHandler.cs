namespace Pi.SetMarketDataWSS.Infrastructure.Interfaces.Kafka;

public interface IKafkaMessageHandler<in T>
{
    Task<(bool processed, bool storedInRedis)> HandleAsync(T message);
}