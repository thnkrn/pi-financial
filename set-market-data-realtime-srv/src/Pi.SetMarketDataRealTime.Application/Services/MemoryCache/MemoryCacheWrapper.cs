using Microsoft.Extensions.Caching.Memory;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;

namespace Pi.SetMarketDataRealTime.Application.Services.MemoryCache;

public class MemoryCacheWrapper : IMemoryCacheWrapper
{
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _defaultOptions;

    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    public MemoryCacheWrapper(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _defaultOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));
    }

    public Task<string?> GetStringAsync(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        return _cache.TryGetValue(key, out string? value) ? Task.FromResult(value) : Task.FromResult<string?>(null);
    }

    public Task SetStringAsync(string key, string value, TimeSpan? expiration = null)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        try
        {
            if (string.IsNullOrEmpty(value)) value = string.Empty;
            var options = expiration.HasValue
                ? new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiration.Value)
                : _defaultOptions;

            _cache.Set(key, value, options);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        _cache.Remove(key);
        return Task.CompletedTask;
    }
}