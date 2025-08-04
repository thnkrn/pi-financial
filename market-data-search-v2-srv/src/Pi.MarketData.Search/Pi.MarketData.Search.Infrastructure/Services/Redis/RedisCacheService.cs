using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.MarketData.Search.Domain.ConstantConfigurations;
using Pi.MarketData.Search.Infrastructure.Converters;
using Pi.MarketData.Search.Infrastructure.Interfaces.Redis;
using StackExchange.Redis;

namespace Pi.MarketData.Search.Infrastructure.Services.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly JsonSerializerSettings _jsonSettings = new() { Converters = [new ObjectIdConverter()] };
    private readonly ILogger<RedisCacheService> _logger;
    private readonly string _redisKeyspace;
    public RedisCacheService(IConfiguration configuration, ILogger<RedisCacheService> logger)
    {
        var config = configuration ?? throw new ArgumentNullException(nameof(configuration));

        _redisKeyspace = config.GetValue<string>(ConfigurationKeys.RedisKeyspace)
                         ?? throw new InvalidOperationException("Redis keyspace is not configured.");

        var connectionString = BuildRedisConnectionString(config);
        var redis = ConnectionMultiplexer.Connect(connectionString);
        _database = redis.GetDatabase();
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
                    if (value.HasValue)
                    {
                        try
                        {
                            return JsonConvert.DeserializeObject<T>(value!, _jsonSettings);
                        }
                        catch (JsonSerializationException)
                        {
                            // If direct deserialization fails, try parsing as string first
                            var stringValue = JsonConvert.DeserializeObject<string>(value!, _jsonSettings);
                            return JsonConvert.DeserializeObject<T>(stringValue!, _jsonSettings);
                        }
                    }
                    return default;
                case RedisType.Hash:
                    var serializedValue = await _database.HashGetAsync(concatKey, "data");
                    if (serializedValue.HasValue)
                    {
                        var unescapedJson = JsonConvert.DeserializeObject<string>(serializedValue!) ?? string.Empty;
                        return JsonConvert.DeserializeObject<T>(unescapedJson, _jsonSettings);
                    }

                    return default;

                case RedisType.List:
                    var listValues = await _database.ListRangeAsync(concatKey);
                    return JsonConvert.DeserializeObject<T>(
                        JsonConvert.SerializeObject(
                            listValues.Select(v => v.ToString()).ToList(),
                            _jsonSettings
                        )
                    );

                case RedisType.Set:
                    var setValues = await _database.SetMembersAsync(concatKey);
                    return JsonConvert.DeserializeObject<T>(
                        JsonConvert.SerializeObject(
                            setValues.Select(v => v.ToString()).ToList(),
                            _jsonSettings
                        )
                    );

                default:
                    return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the cache value with key: {ConcatKey}", concatKey);
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
                // Transform key to use Redis hashtags
                var fullKey = $"{_redisKeyspace}{key}";
                return (RedisKey)fullKey;
            })
            .ToHashSet()
            .ToArray();

        _logger.LogDebug("GetBatchAsync: {ConcatKeys}", string.Join(", ", concatKeys));

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
            _logger.LogError(ex, "An error occurred while getting batch cache values.");
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
            _logger.LogError(ex, "An error occurred while setting the cache value with key: {ConcatKey}", concatKey);
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
            _logger.LogError(ex, "An error occurred while removing the cache value with key: {ConcatKey}", concatKey);
        }
    }

    public static string BuildRedisConnectionString(IConfiguration configuration)
    {
        var host = configuration.GetValue<string>(ConfigurationKeys.RedisHost);
        var port = configuration.GetValue(ConfigurationKeys.RedisPort, 6379);
        var username = configuration.GetValue<string>(ConfigurationKeys.RedisUser);
        var password = configuration.GetValue<string>(ConfigurationKeys.RedisPassword);

        // Construct connection string with specific format
        var connectionString = $"{host}:{port}," +
                               $"name={configuration.GetValue(ConfigurationKeys.RedisClientName, "default-client")}," +
                               $"syncTimeout={configuration.GetValue(ConfigurationKeys.RedisSyncTimeout, 10000)}," +
                               $"connectTimeout={configuration.GetValue(ConfigurationKeys.RedisConnectTimeout, 10000)}," +
                               $"ssl={configuration.GetValue(ConfigurationKeys.RedisSsl, false).ToString().ToLower()}," +
                               $"abortConnect={configuration.GetValue(ConfigurationKeys.RedisAbortOnConnectFail, false).ToString().ToLower()}," +
                               $"connectRetry={configuration.GetValue(ConfigurationKeys.RedisConnectRetry, 3)}," +
                               $"keepAlive={configuration.GetValue(ConfigurationKeys.RedisKeepAlive, 60)}";

        // Add authentication if username and password are provided
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            connectionString += $",user={username},password={password}";

        return connectionString;
    }
}