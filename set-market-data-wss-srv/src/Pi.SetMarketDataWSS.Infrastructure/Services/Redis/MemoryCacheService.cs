using Microsoft.Extensions.Caching.Memory;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Redis;

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

    public async Task<IDictionary<string, T?>> GetBatchAsync<T>(List<string> keys, int timeoutMs = 150)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}