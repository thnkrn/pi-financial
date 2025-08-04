namespace Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Redis;

public interface ICacheService
{
    /// <summary>
    ///     To asynchronously Retreive object from redis cache service by key.
    /// </summary>
    /// <typeparam name="T">The type of cached item</typeparam>
    /// <param name="key"></param>
    /// <returns>
    ///     Result contain cached item of type <typeofparamref name="T" />if found, otherwise, <cnull</c></returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    ///     Asynchronously sets a value in the cache with a specified key and optional expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the value to be cached.</typeparam>
    /// <param name="key">The key for the cached item.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">The optional expiration time for the cached item. If <c>null</c>, the item does not expire.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    ///     Asynchronously removes a cached item by its key.
    /// </summary>
    /// <param name="key">The key of the cached item to be removed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAsync(string key);
}