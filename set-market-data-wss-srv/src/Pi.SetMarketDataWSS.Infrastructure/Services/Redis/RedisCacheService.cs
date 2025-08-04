using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Pi.SetMarketDataWSS.Domain.ConstantConfigurations;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketDataWSS.Infrastructure.Services.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly string _redisKeyspace;

    public RedisCacheService(IDistributedCache distributedCache, IConfiguration configuration)
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

    public async Task<IDictionary<string, T?>> GetBatchAsync<T>(List<string> keys, int timeoutMs = 150)
    {
        if (keys == null || keys.Count == 0)
            throw new ArgumentException("Cache keys cannot be null or empty.", nameof(keys));

        var result = new Dictionary<string, T?>();

        // Initialize result dictionary with default values for all keys
        foreach (var key in keys)
        {
            result[key] = default;
        }

        try
        {
            var concatKeys = keys.Select(key => _redisKeyspace + key).ToList();

            // Create a dictionary to map concatenated keys back to original keys
            var keyMapping = new Dictionary<string, string>();
            for (var i = 0; i < keys.Count; i++)
                keyMapping[concatKeys[i]] = keys[i];

            // Create tasks for each key without passing the token yet
            var tasks = new List<Task<(string key, T? value)>>();

            // Create a cancellation token with a short timeout
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeoutMs)))
            {
                // Create all tasks with the token
                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (var key in concatKeys)
                {
                    // Store the token in a local variable that won't be disposed
                    var token = cts.Token;

                    var task = FetchKeyAsync(key, token);
                    tasks.Add(task);
                }

                // WhenAny approach to process results as they arrive
                var pendingTasks = tasks.ToList();
                while (pendingTasks.Count != 0)
                {
                    // Break out of the loop if cancellation is requested
                    if (cts.Token.IsCancellationRequested)
                        break;

                    // Complete as many tasks as we can within the timeout
                    var completedTask = await Task.WhenAny(pendingTasks);
                    pendingTasks.Remove(completedTask);

                    try
                    {
                        // Add the result of completed task
                        var (key, value) = await completedTask;
                        if (keyMapping.TryGetValue(key, out var originalKey))
                        {
                            result[originalKey] = value;
                        }
                    }
                    catch
                    {
                        // Individual task exception - already handled in FetchKeyAsync
                    }
                }
            } // cts is disposed here

            // Process any remaining completed tasks that finished after timeout
            foreach (var task in tasks.Where(t => t is { IsCompleted: true, IsFaulted: false, IsCanceled: false }))
            {
                try
                {
                    var (key, value) = await task;
                    if (keyMapping.TryGetValue(key, out var originalKey) && !Equals(value, default(T)))
                    {
                        result[originalKey] = value;
                    }
                }
                catch
                {
                    // Ignore errors from remaining tasks
                }
            }

            return result;
        }
        catch (Exception)
        {
            return result;
        }

        // Helper method to fetch a single key
        async Task<(string key, T? value)> FetchKeyAsync(string key, CancellationToken token)
        {
            try
            {
                // Attempt to get value with timeout
                var value = await _distributedCache.GetStringAsync(key, token);
                return (key, value == null ? default : JsonConvert.DeserializeObject<T>(value));
            }
            catch (Exception)
            {
                // Silently fail for individual keys
                return (key, default);
            }
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