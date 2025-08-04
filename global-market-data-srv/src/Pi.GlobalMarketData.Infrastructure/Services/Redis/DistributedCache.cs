using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;


namespace Pi.GlobalMarketData.Infrastructure.Services.Redis;

public class DistributedCache : ICacheService
{
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
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
        catch (Exception ex)
        {
            // Log the exception (logging mechanism should be implemented)
            throw new InvalidOperationException("An error occurred while getting the cache value.", ex);
        }
    }

    public async Task<IDictionary<string, T?>> GetBatchAsync<T>(List<string> keys)
    {
        if (keys == null || keys.Count == 0)
            throw new ArgumentException("Cache keys cannot be null or empty.", nameof(keys));

        var result = new Dictionary<string, T?>();
        var concatKeys = keys.Select(key => _redisKeyspace + key).ToList();

        try
        {
            // Create a dictionary to map concatenated keys back to original keys
            var keyMapping = new Dictionary<string, string>();
            for (var i = 0; i < keys.Count; i++) keyMapping[concatKeys[i]] = keys[i];

            // Fetch all values in parallel
            var values = await Task.WhenAll(concatKeys.Select(key => _distributedCache.GetStringAsync(key)));

            // Process results
            for (var i = 0; i < concatKeys.Count; i++)
            {
                var originalKey = keyMapping[concatKeys[i]];
                var value = values[i];
                result[originalKey] = value == null ? default : JsonConvert.DeserializeObject<T>(value);
            }

            return result;
        }
        catch (Exception ex)
        {
            // Log the exception (logging mechanism should be implemented)
            throw new InvalidOperationException("An error occurred while getting batch cache values.", ex);
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