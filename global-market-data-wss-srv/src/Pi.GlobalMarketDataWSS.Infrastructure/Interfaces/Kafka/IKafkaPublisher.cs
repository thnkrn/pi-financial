namespace Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Kafka;

public interface IKafkaPublisher
{
    Task PublishAsync<T>(string topic, T message);
}