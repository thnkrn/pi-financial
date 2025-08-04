namespace Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task<IDictionary<string, T?>> GetBatchAsync<T>(List<string> keys);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
}