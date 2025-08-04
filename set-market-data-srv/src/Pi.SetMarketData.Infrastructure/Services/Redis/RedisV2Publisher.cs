using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Exceptions;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;
using StackExchange.Redis;

namespace Pi.SetMarketData.Infrastructure.Services.Redis;

public sealed class RedisV2Publisher : IRedisV2Publisher, IDisposable, IAsyncDisposable
{
    private const string Field = "data";
    private const string CacheKeyError = "Cache key cannot be null or empty.";

    private readonly IRedisConnectionProvider _connectionProvider;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _keyspace;
    private bool _disposed;

    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="connectionProvider"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public RedisV2Publisher(IConfiguration configuration, IRedisConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        _keyspace = configuration[ConfigurationKeys.RedisKeyspace] ?? "marketdata::";
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

    public async Task<T?> GetAsync<T>(string key, bool compress = false)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(CacheKeyError, nameof(key));

        var originalKey = key;

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
                    if (value.HasValue)
                    {
                        var decompressedValue = CompressionHelper.DecompressData(value);
                        return JsonSerializer.Deserialize<T>(decompressedValue, _jsonOptions);
                    }
                }
                catch (CompressionException)
                {
                    await RemoveAsync(originalKey);
                    return default;
                }
                catch (Exception ex) when (ex is not CompressionException)
                {
                    return JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
                }

            return JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException($"An error occurred while operation with the hash key '{key}'", ex);
        }
    }

    public async Task SetAsync<T>(string key, T value, bool compress = false, TimeSpan? expiration = null)
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
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException($"An error occurred while operation with the key '{key}'", ex);
        }
    }

    public async Task SetStringAsync<T>(string key, T value, bool compress = false, TimeSpan? expiration = null)
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
                await db.StringSetAsync(key, compressedBytes);
            }
            else
            {
                await db.StringSetAsync(key, json);
            }

            if (expiration.HasValue)
                await db.KeyExpireAsync(key, expiration);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException($"An error occurred while operation with the key '{key}'", ex);
        }
    }

    [SuppressMessage("SonarQube", "S3776:Cognitive Complexity of methods should not be too high")]
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
            // Process in batches of up to 50 (adjust this value based on your use case)
            var batchSize = keys.Count;
            for (var i = 0; i < originalToRedisKeyMap.Count; i += batchSize)
            {
                var batch = originalToRedisKeyMap.Skip(i).Take(batchSize).ToList();

                var db = _connectionProvider.GetDatabase();

                // Create a batch of commands
                var batchExe = db.CreateBatch();

                // Prepare tasks for all hash gets in this batch
                var tasks = new Dictionary<string, Task<RedisValue>>();
                foreach (var (originalKey, redisKey) in batch)
                    tasks[originalKey] = batchExe.HashGetAsync(redisKey, Field);

                // Execute batch
                batchExe.Execute();

                // Process results as they complete
                var processingTasks = new List<Task>();
                foreach (var (originalKey, valueTask) in tasks)
                    processingTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            var value = await valueTask;
                            if (value == RedisValue.Null)
                            {
                                lock (result)
                                {
                                    result[originalKey] = default;
                                }

                                return;
                            }

                            if (compress)
                            {
                                try
                                {
                                    var decompressedValue = CompressionHelper.DecompressData(value);
                                    var deserializedValue =
                                        JsonSerializer.Deserialize<T>(decompressedValue, _jsonOptions);
                                    lock (result)
                                    {
                                        result[originalKey] = deserializedValue;
                                    }
                                }
                                catch (CompressionException)
                                {
                                    await RemoveAsync(originalKey);
                                    lock (result)
                                    {
                                        result[originalKey] = default;
                                    }
                                }
                                catch (Exception ex) when (ex is not CompressionException)
                                {
                                    var deserializedValue =
                                        JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
                                    lock (result)
                                    {
                                        result[originalKey] = deserializedValue;
                                    }
                                }
                            }
                            else
                            {
                                var deserializedValue =
                                    JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
                                lock (result)
                                {
                                    result[originalKey] = deserializedValue;
                                }
                            }
                        }
                        catch
                        {
                            lock (result)
                            {
                                result[originalKey] = default;
                            }
                        }
                    }));

                // Wait for all processing to complete
                await Task.WhenAll(processingTasks);
            }

            return result;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException("An error occurred while getting multiple cache values.", ex);
        }
    }

    [SuppressMessage("SonarQube", "S3776:Cognitive Complexity of methods should not be too high")]
    public async Task<IDictionary<string, T?>> GetManyAsync<T>(Dictionary<string, bool> keys)
    {
        ThrowIfDisposed();

        var result = new Dictionary<string, T?>();

        if (keys.Count == 0)
            return result;

        try
        {
            // Process in batches of up to 50 (adjust this value based on your use case)
            var batchSize = keys.Count;
            var keysList = keys.ToList();

            for (var i = 0; i < keysList.Count; i += batchSize)
            {
                var batchKeys = keysList.Skip(i).Take(batchSize).ToList();

                var db = _connectionProvider.GetDatabase();

                // Create batch
                var batch = db.CreateBatch();

                // Set up tasks for all keys in batch
                var tasks = new List<(string Key, bool Compress, Task<RedisValue> Task)>();

                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (var kvp in batchKeys)
                {
                    var redisKey = !string.IsNullOrEmpty(_keyspace)
                        ? $"{_keyspace}{kvp.Key}"
                        : kvp.Key;

                    tasks.Add((kvp.Key, kvp.Value, batch.HashGetAsync(redisKey, Field)));
                }

                // Execute batch
                batch.Execute();

                // Process results
                var processingTasks = tasks.Select(async item =>
                {
                    try
                    {
                        var value = await item.Task;

                        if (value == RedisValue.Null)
                        {
                            lock (result)
                            {
                                result[item.Key] = default;
                            }

                            return;
                        }

                        try
                        {
                            if (item.Compress)
                            {
                                try
                                {
                                    var decompressedValue = CompressionHelper.DecompressData(value);
                                    var deserializedValue =
                                        JsonSerializer.Deserialize<T>(decompressedValue, _jsonOptions);
                                    lock (result)
                                    {
                                        result[item.Key] = deserializedValue;
                                    }
                                }
                                catch (CompressionException)
                                {
                                    await RemoveAsync(item.Key);
                                    lock (result)
                                    {
                                        result[item.Key] = default;
                                    }
                                }
                                catch (Exception ex) when (ex is not CompressionException)
                                {
                                    var deserializedValue =
                                        JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
                                    lock (result)
                                    {
                                        result[item.Key] = deserializedValue;
                                    }
                                }
                            }
                            else
                            {
                                var deserializedValue =
                                    JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
                                lock (result)
                                {
                                    result[item.Key] = deserializedValue;
                                }
                            }
                        }
                        catch
                        {
                            lock (result)
                            {
                                result[item.Key] = default;
                            }
                        }
                    }
                    catch
                    {
                        lock (result)
                        {
                            result[item.Key] = default;
                        }
                    }
                }).ToArray();

                // Wait for all processing to complete
                await Task.WhenAll(processingTasks);
            }

            return result;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException("An error occurred while getting multiple cache values.", ex);
        }
    }

    public async Task<bool> RemoveAsync(string key)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(CacheKeyError, nameof(key));

        if (!string.IsNullOrEmpty(_keyspace))
            key = $"{_keyspace}{key}";

        try
        {
            var db = _connectionProvider.GetDatabase();
            return await db.KeyDeleteAsync(key);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException($"An error occurred while removing key '{key}'", ex);
        }
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