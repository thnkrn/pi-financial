using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Pi.SetMarketDataWSS.Domain.ConstantConfigurations;
using Pi.SetMarketDataWSS.Infrastructure.Exceptions;
using Pi.SetMarketDataWSS.Infrastructure.Helpers;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Redis;
using StackExchange.Redis;

namespace Pi.SetMarketDataWSS.Infrastructure.Services.Redis;

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

    public async Task SetAsync<T>(string key, T value, bool compress = false, TimeSpan? expiration = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(CacheKeyError, nameof(key));

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
            throw new ArgumentException(CacheKeyError, nameof(key));

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

    [SuppressMessage("SonarQube", "S3776")]
    public async Task<IDictionary<string, T?>> GetManyAsync<T>(List<string> keys, bool compress = false)
    {
        ThrowIfDisposed();

        if (keys.Count == 0)
            return new Dictionary<string, T?>();

        var result = new Dictionary<string, T?>();
        var originalToRedisKeyMap = new Dictionary<string, string>();

        MapOriginalKeyToRedisKey(keys, originalToRedisKeyMap);

        if (originalToRedisKeyMap.Count == 0)
            return result;

        try
        {
            const int batchSize = 20; // Consistent batch size with the other method

            for (var i = 0; i < originalToRedisKeyMap.Count; i += batchSize)
            {
                var batchKeys = originalToRedisKeyMap.Skip(i).Take(batchSize).ToList();
                var batchResult = new ConcurrentDictionary<string, T?>();
                var db = _connectionProvider.GetDatabase();

                // Create batch
                var batch = db.CreateBatch();

                // Set up tasks for all keys in batch
                var tasks = new List<(string Key, Task<RedisValue> Task)>();

                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (var kvp in batchKeys)
                    tasks.Add((kvp.Key, batch.HashGetAsync(kvp.Value, Field)));

                try
                {
                    // Execute batch
                    batch.Execute();
                }
                catch (Exception)
                {
                    // If batch execution fails, mark all keys in this batch as default
                    foreach (var task in tasks)
                        batchResult[task.Key] = default;

                    // Add batch results to the main result
                    foreach (var item in batchResult)
                        result[item.Key] = item.Value;

                    continue; // Skip to the next batch
                }

                // Process results independently - each key failure is isolated
                var processingTasks = tasks.Select(async item =>
                {
                    try
                    {
                        var value = await item.Task;

                        if (value.IsNull)
                        {
                            batchResult[item.Key] = default;
                            return;
                        }

                        // Process compressed data
                        if (compress)
                            try
                            {
                                var decompressedValue = CompressionHelper.DecompressData(value);
                                var deserializedValue =
                                    JsonSerializer.Deserialize<T>(decompressedValue, _jsonOptions);
                                batchResult[item.Key] = deserializedValue;
                            }
                            catch (CompressionException)
                            {
                                try
                                {
                                    await RemoveAsync(item.Key);
                                }
                                catch (Exception)
                                {
                                    // Nothing to do
                                }

                                batchResult[item.Key] = default;
                            }
                            catch (Exception)
                            {
                                // Fallback to direct deserialization
                                try
                                {
                                    var deserializedValue =
                                        JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
                                    batchResult[item.Key] = deserializedValue;
                                }
                                catch (Exception)
                                {
                                    batchResult[item.Key] = default;
                                }
                            }
                        else
                            try
                            {
                                var deserializedValue =
                                    JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
                                batchResult[item.Key] = deserializedValue;
                            }
                            catch (Exception)
                            {
                                batchResult[item.Key] = default;
                            }
                    }
                    catch (Exception)
                    {
                        batchResult[item.Key] = default;
                    }
                }).ToArray();

                // Wait for all processing to complete
                await Task.WhenAll(processingTasks);

                // Add batch results to the main result
                foreach (var item in batchResult) result[item.Key] = item.Value;
            }

            return result;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException("An error occurred while getting multiple cache values.", ex);
        }
    }

    [SuppressMessage("SonarQube", "S3776")]
    public async Task<IDictionary<string, T?>> GetManyAsync<T>(Dictionary<string, bool> keys)
    {
        ThrowIfDisposed();

        var result = new Dictionary<string, T?>();

        if (keys.Count == 0)
            return result;

        try
        {
            const int batchSize = 20;
            var keysList = keys.ToList();

            for (var i = 0; i < keysList.Count; i += batchSize)
            {
                var batchKeys = keysList.Skip(i).Take(batchSize).ToList();
                var batchResult = new ConcurrentDictionary<string, T?>();
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

                try
                {
                    // Execute batch
                    batch.Execute();
                }
                catch (Exception)
                {
                    // If batch execution fails, mark all keys in this batch as default
                    // but don't affect other batches that might succeed
                    foreach (var task in tasks)
                        batchResult[task.Key] = default;

                    // Add batch results to the main result
                    foreach (var item in batchResult)
                        result[item.Key] = item.Value;

                    continue; // Skip to the next batch
                }

                // Process results independently - each key failure is isolated
                var processingTasks = tasks.Select(async item =>
                {
                    try
                    {
                        var value = await item.Task;

                        if (value.IsNull)
                        {
                            batchResult[item.Key] = default;
                            return;
                        }

                        // Process compressed data
                        if (item.Compress)
                            try
                            {
                                var decompressedValue = CompressionHelper.DecompressData(value);
                                var deserializedValue =
                                    JsonSerializer.Deserialize<T>(decompressedValue, _jsonOptions);
                                batchResult[item.Key] = deserializedValue;
                            }
                            catch (CompressionException)
                            {
                                try
                                {
                                    await RemoveAsync(item.Key);
                                }
                                catch (Exception)
                                {
                                    // Nothing to do
                                }

                                batchResult[item.Key] = default;
                            }
                            catch (Exception)
                            {
                                // Fallback to direct deserialization
                                try
                                {
                                    var deserializedValue =
                                        JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
                                    batchResult[item.Key] = deserializedValue;
                                }
                                catch (Exception)
                                {
                                    batchResult[item.Key] = default;
                                }
                            }
                        else
                            try
                            {
                                var deserializedValue =
                                    JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
                                batchResult[item.Key] = deserializedValue;
                            }
                            catch (Exception)
                            {
                                batchResult[item.Key] = default;
                            }
                    }
                    catch (Exception)
                    {
                        batchResult[item.Key] = default;
                    }
                }).ToArray();

                // Wait for all processing to complete
                await Task.WhenAll(processingTasks);

                // Add batch results to the main result
                foreach (var item in batchResult) result[item.Key] = item.Value;
            }

            return result;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException("An error occurred while getting multiple cache values.", ex);
        }
    }

    public async Task AddSortedSetAsync(string key, string member, double score)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(CacheKeyError, nameof(key));

        if (!string.IsNullOrEmpty(_keyspace))
            key = $"{_keyspace}{key}";

        try
        {
            var db = _connectionProvider.GetDatabase();
            await db.SortedSetAddAsync(key, member, score);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException($"An error occurred while adding to sorted set with key '{key}'", ex);
        }
    }

    public async Task<IEnumerable<SortedSetEntry>> GetSortedSetWithScoresAsync(string key, int start, int end)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(CacheKeyError, nameof(key));

        if (!string.IsNullOrEmpty(_keyspace))
            key = $"{_keyspace}{key}";

        try
        {
            var db = _connectionProvider.GetDatabase();
            var entries = await db.SortedSetRangeByRankWithScoresAsync(key, start, end);
            return entries;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException($"An error occurred while getting sorted set with key '{key}'", ex);
        }
    }

    public async Task<bool> KeyExistsAsync(string key)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(CacheKeyError, nameof(key));

        if (!string.IsNullOrEmpty(_keyspace))
            key = $"{_keyspace}{key}";

        try
        {
            var db = _connectionProvider.GetDatabase();
            return await db.KeyExistsAsync(key);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException($"An error occurred while checking key existence '{key}'", ex);
        }
    }

    public async Task HashSetAsync(string key, string field, string value)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(CacheKeyError, nameof(key));

        if (!string.IsNullOrEmpty(_keyspace))
            key = $"{_keyspace}{key}";

        try
        {
            var db = _connectionProvider.GetDatabase();
            await db.HashSetAsync(key, field, value);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException($"An error occurred while setting hash field '{key}:{field}'", ex);
        }
    }

    public async Task<Dictionary<string, string>> HashGetAllAsync(string key)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(CacheKeyError, nameof(key));

        if (!string.IsNullOrEmpty(_keyspace))
            key = $"{_keyspace}{key}";

        try
        {
            var db = _connectionProvider.GetDatabase();
            var hashEntries = await db.HashGetAllAsync(key);
            return hashEntries.ToDictionary(
                he => he.Name.ToString(),
                he => he.Value.ToString());
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException($"An error occurred while getting all hash fields for key '{key}'", ex);
        }
    }

    public async Task<long> SortedSetLengthAsync(string key)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(CacheKeyError, nameof(key));

        if (!string.IsNullOrEmpty(_keyspace))
            key = $"{_keyspace}{key}";

        try
        {
            var db = _connectionProvider.GetDatabase();
            return await db.SortedSetLengthAsync(key);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException($"An error occurred while getting sorted set length for key '{key}'",
                ex);
        }
    }

    public async Task SortedSetRemoveRangeByRankAsync(string key, long start, long stop)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(CacheKeyError, nameof(key));

        if (!string.IsNullOrEmpty(_keyspace))
            key = $"{_keyspace}{key}";

        try
        {
            var db = _connectionProvider.GetDatabase();
            await db.SortedSetRemoveRangeByRankAsync(key, start, stop);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException($"An error occurred while removing range from sorted set '{key}'", ex);
        }
    }

    public IRedisTransaction CreateTransaction()
    {
        ThrowIfDisposed();

        return new RedisTransaction(_connectionProvider.GetDatabase(), _keyspace);
    }

    public async Task<bool> ExecuteTransactionAsync(IRedisTransaction transaction)
    {
        ThrowIfDisposed();

        if (transaction is RedisTransaction redisTransaction)
            return await redisTransaction.ExecuteAsync();

        throw new ArgumentException("Invalid transaction type", nameof(transaction));
    }

    public async Task<bool> HashSetManyAsync(string key, IDictionary<string, string> fieldValues)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(CacheKeyError, nameof(key));

        if (!string.IsNullOrEmpty(_keyspace))
            key = $"{_keyspace}{key}";

        try
        {
            var db = _connectionProvider.GetDatabase();
            var hashEntries = fieldValues.Select(kv => new HashEntry(kv.Key, kv.Value)).ToArray();
            await db.HashSetAsync(key, hashEntries);
            return true;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException($"An error occurred while setting multiple hash fields for key '{key}'",
                ex);
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

internal class RedisTransaction : IRedisTransaction
{
    private readonly string _keyspace;
    private readonly ITransaction _transaction;

    /// <summary>
    /// </summary>
    /// <param name="database"></param>
    /// <param name="keyspace"></param>
    public RedisTransaction(IDatabase database, string keyspace)
    {
        _transaction = database.CreateTransaction();
        _keyspace = keyspace;
    }

    public IRedisTransaction HashSet(string key, string field, string value)
    {
        key = $"{_keyspace}{key}";
        _transaction.HashSetAsync(key, field, value);
        return this;
    }

    public IRedisTransaction SortedSetAdd(string key, string member, long score)
    {
        key = $"{_keyspace}{key}";
        _transaction.SortedSetAddAsync(key, member, score);
        return this;
    }

    public async Task<bool> ExecuteAsync()
    {
        return await _transaction.ExecuteAsync();
    }
}