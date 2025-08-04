namespace Pi.SetMarketData.Infrastructure.Interfaces.Redis;

public interface IRedisSubscriber;

public interface IRedisV2Publisher
{
    Task PublishAsync<T>(string channel, T message, bool compress = false);
    Task<T?> GetAsync<T>(string key, bool compress = false);
    Task<IDictionary<string, T?>> GetManyAsync<T>(List<string> keys, bool compress = false);
    Task<IDictionary<string, T?>> GetManyAsync<T>(Dictionary<string, bool> keys);
    Task SetAsync<T>(string key, T value, bool compress = false, TimeSpan? expiration = null);
    Task SetStringAsync<T>(string key, T value, bool compress = false, TimeSpan? expiration = null);
    Task<bool> RemoveAsync(string key);
}