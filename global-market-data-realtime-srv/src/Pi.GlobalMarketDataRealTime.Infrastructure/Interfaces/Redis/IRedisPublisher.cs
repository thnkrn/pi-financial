namespace Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Redis;

public interface IRedisPublisher
{
    Task PublishAsync<T>(string channel, T message);
}

public interface IRedisV2Publisher
{
    Task PublishAsync<T>(string channel, T message, bool compress = false);
    Task<T?> GetAsync<T>(string key, bool compress = false);
    Task<bool> SetAsync<T>(string key, T value, bool compress = false, TimeSpan? expiration = null);
    Task<bool> RemoveAsync(string key);
}