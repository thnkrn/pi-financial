namespace Pi.SetMarketData.Infrastructure.Interfaces.Redis;

public interface IRedisPublisher
{
    Task PublishAsync<T>(string channel, T message);
}