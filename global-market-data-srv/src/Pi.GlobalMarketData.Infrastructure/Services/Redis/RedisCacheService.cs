using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using StackExchange.Redis;

namespace Pi.GlobalMarketData.Infrastructure.Services.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly string _redisKeyspace;

    public RedisCacheService(IConfiguration configuration, ILogger<RedisCacheService> logger,
        IRedisConnectionProvider connectionProvider)
    {
        var config = configuration ?? throw new ArgumentNullException(nameof(configuration));

        _redisKeyspace = config.GetValue<string>(ConfigurationKeys.RedisKeyspace)
                         ?? throw new InvalidOperationException("Redis keyspace is not configured.");

        _database = connectionProvider.GetDatabase();
        _logger = logger;
    }


    public async Task<T?> GetAsync<T>(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

        var concatKey = _redisKeyspace + key;
        try
        {
            if (!await _database.KeyExistsAsync(concatKey)) return default;

            var keyType = await _database.KeyTypeAsync(concatKey);

            switch (keyType)
            {
                case RedisType.String:
                    var value = await _database.StringGetAsync(concatKey);
                    return value.HasValue
                        ? JsonConvert.DeserializeObject<T>(value!)
                        : default;

                case RedisType.Hash:
                    var serializedValue = await _database.HashGetAsync(concatKey, "data");
                    return serializedValue.HasValue ? JsonConvert.DeserializeObject<T>(serializedValue!) : default;
                case RedisType.List:
                    var listValues = await _database.ListRangeAsync(concatKey);
                    return JsonConvert.DeserializeObject<T>(
                        JsonConvert.SerializeObject(
                            listValues.Select(v => v.ToString()).ToList()
                        )
                    );

                case RedisType.Set:
                    var setValues = await _database.SetMembersAsync(concatKey);
                    return JsonConvert.DeserializeObject<T>(
                        JsonConvert.SerializeObject(
                            setValues.Select(v => v.ToString()).ToList()
                        )
                    );

                default:
                    return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the cache value with key: {Key}\n{Ex}", concatKey,
                ex.Message);
            return default;
        }
    }

    public async Task<IDictionary<string, T?>> GetBatchAsync<T>(List<string> keys)
    {
        var results = new Dictionary<string, T?>();
        if (keys.Count == 0)
            return results;

        var concatKeys = keys.Select(key =>
            {
                var transformedKey = $"{_redisKeyspace}{key}";
                return (RedisKey)transformedKey;
            })
            .ToHashSet()
            .ToArray();

        try
        {
            var retrievedValues = await _database.StringGetAsync(concatKeys);
            results = concatKeys.Zip(retrievedValues, (key, value) => new { Key = key, Value = value })
                .ToDictionary(
                    x => x.Key.ToString()
                        .Replace(_redisKeyspace, string.Empty),
                    x => x.Value.HasValue
                        ? JsonConvert.DeserializeObject<T>(x.Value!)
                        : default
                );
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting batch cache values. {Ex}", ex.Message);
            return results;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

        var concatKey = _redisKeyspace + key;

        try
        {
            var serializedValue = JsonConvert.SerializeObject(value);
            await _database.StringSetAsync(concatKey, serializedValue);
            if (expiration.HasValue) await _database.KeyExpireAsync(concatKey, expiration.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while setting the cache value with key: {Key}\n{Ex}", concatKey,
                ex.Message);
        }
    }

    public async Task RemoveAsync(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

        var concatKey = _redisKeyspace + key;
        try
        {
            await _database.KeyDeleteAsync(concatKey);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"An error occurred while removing the cache value with key: {key}",
                ex);
        }
    }
}