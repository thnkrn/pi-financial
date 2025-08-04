using Microsoft.Extensions.Caching.Memory;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.GlobalMarketData.Infrastructure.Services.Redis;
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        return Task.FromResult(_memoryCache.TryGetValue(key, out T value) ? value : default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue) options.SetAbsoluteExpiration(expiration.Value);
        _memoryCache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public Task<IDictionary<string, T?>> GetBatchAsync<T>(List<string> keys)
    {
        throw new NotImplementedException();
    }
}