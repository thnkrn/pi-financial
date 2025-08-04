using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Converters;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketData.Infrastructure.Services.Redis;

public class DistributedCache : ICacheService
{
    private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings { Converters = [new ObjectIdConverter()] };
    private readonly IDistributedCache _distributedCache;
    private readonly string _redisKeyspace;

    public DistributedCache(IDistributedCache distributedCache, IConfiguration configuration)
    {
        var config = configuration ?? throw new ArgumentNullException(nameof(configuration));

        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        _redisKeyspace = config.GetValue<string>(ConfigurationKeys.RedisKeyspace)
                         ?? throw new InvalidOperationException("Redis keyspace is not configured.");
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

        var concatKey = _redisKeyspace + key;
        try
        {
            var value = await _distributedCache.GetStringAsync(concatKey);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value, _jsonSettings);
        }
        catch (Exception ex)
        {
            // Log the exception (logging mechanism should be implemented)
            throw new InvalidOperationException("An error occurred while getting the cache value.", ex);
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

        var concatKey = _redisKeyspace + key;

        try
        {
            var options = new DistributedCacheEntryOptions();
            if (expiration.HasValue) options.SetAbsoluteExpiration(expiration.Value);
            var serializedValue = JsonConvert.SerializeObject(value);
            await _distributedCache.SetStringAsync(concatKey, serializedValue, options);
        }
        catch (Exception ex)
        {
            // Log the exception (logging mechanism should be implemented)
            throw new InvalidOperationException("An error occurred while setting the cache value.", ex);
        }
    }

    public Task<IDictionary<string, T?>> GetBatchAsync<T>(List<string> keys)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

        var concatKey = _redisKeyspace + key;
        try
        {
            await _distributedCache.RemoveAsync(concatKey);
        }
        catch (Exception ex)
        {
            // Log the exception (logging mechanism should be implemented)
            throw new InvalidOperationException("An error occurred while removing the cache value.", ex);
        }
    }   
}