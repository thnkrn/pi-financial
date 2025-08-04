using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Infrastructure.Helpers;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Redis;
using StackExchange.Redis;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Redis;

public sealed class RedisV2Publisher : IRedisV2Publisher, IDisposable, IAsyncDisposable
{
    private const string Field = "data";
    private readonly IRedisConnectionProvider _connectionProvider;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _keyspace;
    private readonly ILogger<RedisV2Publisher> _logger;
    private bool _disposed;

    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="connectionProvider"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public RedisV2Publisher(IConfiguration configuration, ILogger<RedisV2Publisher> logger,
        IRedisConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        _keyspace = configuration[ConfigurationKeys.RedisKeyspace] ?? "marketdata::";
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Pre-compute JsonSerializerOptions
        _jsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await DisposeAsyncCore();

        Dispose(false);
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task PublishAsync<T>(string channel, T message, bool compress = false)
    {
        ThrowIfDisposed();

        if (!string.IsNullOrEmpty(_keyspace))
            channel = $"{_keyspace}{channel}";

        await PublishDirectAsync(channel, message, compress);
    }

    public async Task<bool> SetAsync<T>(string key, T value, bool compress = false, TimeSpan? expiration = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

        if (!string.IsNullOrEmpty(_keyspace))
            key = $"{_keyspace}{key}";

        try
        {
            var db = _connectionProvider.GetDatabase();
            var json = JsonSerializer.Serialize(value, _jsonOptions);

            if (compress)
            {
                var compressedBytes = CompressionHelper.CompressString(json);
                await db.HashSetAsync(key, Field, compressedBytes);
            }
            else
            {
                await db.HashSetAsync(key, Field, json);
            }

            if (expiration.HasValue)
                await db.KeyExpireAsync(key, expiration);

            return true;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "An error occurred while operation with the key '{Key}'", key);
            return true;
        }
    }

    public async Task<T?> GetAsync<T>(string key, bool compress = false)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

        if (!string.IsNullOrEmpty(_keyspace))
            key = $"{_keyspace}{key}";

        try
        {
            var db = _connectionProvider.GetDatabase();
            var value = await db.HashGetAsync(key, Field);

            if (value == RedisValue.Null)
                return default;

            if (compress)
                try
                {
                    if (value.HasValue && value.ToString().Length > 0)
                    {
                        var decompressedValue = CompressionHelper.DecompressData(value);
                        return JsonSerializer.Deserialize<T>(decompressedValue, _jsonOptions);
                    }
                }
                catch (Exception)
                {
                    return JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
                }

            return JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "An error occurred while operation with the key '{Key}'", key);
            return default;
        }
    }

    public async Task<bool> RemoveAsync(string key)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

        if (!string.IsNullOrEmpty(_keyspace))
            key = $"{_keyspace}{key}";

        try
        {
            var db = _connectionProvider.GetDatabase();
            return await db.KeyDeleteAsync(key);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "An error occurred while operation with the key '{Key}'", key);
            return false;
        }
    }

    public async Task<IDictionary<string, T?>> GetManyAsync<T>(List<string> keys, bool compress = false)
    {
        ThrowIfDisposed();

        var result = new Dictionary<string, T?>();
        var originalToRedisKeyMap = new Dictionary<string, string>();

        MapOriginalKeyToRedisKey(keys, originalToRedisKeyMap);

        if (originalToRedisKeyMap.Count == 0)
            return result;

        try
        {
            var db = _connectionProvider.GetDatabase();

            foreach (var originalKey in originalToRedisKeyMap.Keys)
            {
                var redisKey = originalToRedisKeyMap[originalKey];
                var value = await db.HashGetAsync(redisKey, Field);

                if (value == RedisValue.Null)
                {
                    result[originalKey] = default;
                    continue;
                }

                try
                {
                    if (compress)
                        DecompressData(value, result, originalKey);
                    else
                        result[originalKey] = JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
                }
                catch (Exception)
                {
                    result[originalKey] = default;
                }
            }

            return result;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "An error occurred while operation");
            return new Dictionary<string, T?>();
        }
    }

    private void DecompressData<T>(RedisValue value, Dictionary<string, T?> result, string originalKey)
    {
        if (value.HasValue)
            try
            {
                var decompressedValue = CompressionHelper.DecompressData(value);
                result[originalKey] =
                    JsonSerializer.Deserialize<T>(decompressedValue, _jsonOptions);
            }
            catch
            {
                result[originalKey] = JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
            }
        else
            result[originalKey] = default;
    }

    private void MapOriginalKeyToRedisKey(List<string> keys, Dictionary<string, string> originalToRedisKeyMap)
    {
        foreach (var originalKey in keys)
        {
            if (string.IsNullOrEmpty(originalKey))
                continue;

            var redisKey = !string.IsNullOrEmpty(_keyspace)
                ? $"{_keyspace}{originalKey}"
                : originalKey;

            originalToRedisKeyMap[originalKey] = redisKey;
        }
    }

    private async Task PublishDirectAsync<T>(string channel, T message, bool compress)
    {
        var subscriber = _connectionProvider.GetConnection().GetSubscriber();
        var json = JsonSerializer.Serialize(message, _jsonOptions);

        if (compress)
        {
            // Use CompressionHelper to compress the JSON string
            var compressedBytes = CompressionHelper.CompressString(json);
            await subscriber.PublishAsync(RedisChannel.Literal(channel), compressedBytes);
        }
        else
        {
            await subscriber.PublishAsync(RedisChannel.Literal(channel), json);
        }
    }

    private static async ValueTask DisposeAsyncCore()
    {
        // Nothing to do for _connectionProvider
        await Task.CompletedTask;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // Nothing to do for _connectionProvider
        }

        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RedisV2Publisher));
    }

    ~RedisV2Publisher()
    {
        Dispose(false);
    }
}