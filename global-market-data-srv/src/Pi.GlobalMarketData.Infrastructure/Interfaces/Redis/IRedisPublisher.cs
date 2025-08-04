namespace Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;

public interface IRedisPublisher
{
    Task PublishAsync<T>(string channel, T message);
}