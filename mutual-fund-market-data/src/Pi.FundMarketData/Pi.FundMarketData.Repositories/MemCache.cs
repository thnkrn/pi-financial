using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Repositories;

public class MemCache : ICache
{
    private readonly MemoryCache _memoryCache;
    private readonly DataCacheConfig _cacheConfig;
    private const string _amcKey = "AMC_PROFILE";

    public MemCache(IOptions<DataCacheConfig> options)
    {
        _cacheConfig = options.Value ?? throw new ArgumentNullException(nameof(options));
        var cacheOptions = Options.Create(new MemoryCacheOptions());
        _memoryCache = new MemoryCache(cacheOptions);
    }

    public bool TryGetAmcProfile(string amcCode, out AmcProfile value)
    {
        value = null;
        return TryGetAmcProfiles(out var amcProfiles)
               && amcProfiles.TryGetValue(amcCode, out value);
    }

    public bool TryGetAmcProfiles(out Dictionary<string, AmcProfile> value)
    {
        return TryGet(_amcKey, out value);
    }

    public void AddAmcProfiles(Dictionary<string, AmcProfile> value)
    {
        Add(_amcKey, value);
    }

    private bool TryGet<T>(string key, out T result)
    {
        result = default;
        return _memoryCache.TryGetValue(key, out result);
    }

    private void Add<T>(string key, T value)
    {
        if (!Equals(value, default(T)))
            _memoryCache.Set(key, value, _cacheConfig.AmcProfileCacheDuration);
    }
}
