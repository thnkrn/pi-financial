using StackExchange.Redis;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Redis;

public interface IRedisPublisher
{
    Task PublishAsync<T>(string channel, T message, bool compress = false);
}

public interface IRedisV2Publisher
{
    Task PublishAsync<T>(string channel, T message, bool compress = false);
    Task<T?> GetAsync<T>(string key, bool compress = false);
    Task<IDictionary<string, T?>> GetManyAsync<T>(List<string> keys, bool compress = false);
    Task<IDictionary<string, T?>> GetManyAsync<T>(Dictionary<string, bool> keys);
    Task SetAsync<T>(string key, T value, bool compress = false, TimeSpan? expiration = null);
    Task SetStringAsync<T>(string key, T value, bool compress = false, TimeSpan? expiration = null);
    Task<bool> RemoveAsync(string key);
    Task AddSortedSetAsync(string key, string member, double score);
    Task<IEnumerable<SortedSetEntry>> GetSortedSetWithScoresAsync(string key, int start, int end);
    Task<bool> KeyExistsAsync(string key);
    Task HashSetAsync(string key, string field, string value);
    Task<Dictionary<string, string>> HashGetAllAsync(string key);
    Task<long> SortedSetLengthAsync(string key);
    Task SortedSetRemoveRangeByRankAsync(string key, long start, long stop);
    IRedisTransaction CreateTransaction();
    Task<bool> ExecuteTransactionAsync(IRedisTransaction transaction);
    Task<bool> HashSetManyAsync(string key, IDictionary<string, string> fieldValues);
}

public interface IRedisTransaction
{
    IRedisTransaction HashSet(string key, string field, string value);
    IRedisTransaction SortedSetAdd(string key, string member, long score);
}