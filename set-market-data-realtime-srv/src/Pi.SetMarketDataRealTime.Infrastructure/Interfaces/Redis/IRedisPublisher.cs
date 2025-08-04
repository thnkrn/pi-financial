namespace Pi.SetMarketDataRealTime.Infrastructure.Interfaces.Redis;

public interface IRedisPublisher
{
    Task PublishAsync<T>(string channel, T message);
}