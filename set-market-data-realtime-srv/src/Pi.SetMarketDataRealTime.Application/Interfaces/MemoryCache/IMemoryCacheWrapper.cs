namespace Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;

public interface IMemoryCacheWrapper
{
    Task<string?> GetStringAsync(string key);
    Task SetStringAsync(string key, string value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
}