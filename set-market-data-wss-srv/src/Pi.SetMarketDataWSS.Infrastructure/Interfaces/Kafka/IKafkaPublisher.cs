namespace Pi.SetMarketDataWSS.Infrastructure.Interfaces.Kafka;

public interface IKafkaPublisher
{
    Task PublishAsync<T>(string topic, T message);
}