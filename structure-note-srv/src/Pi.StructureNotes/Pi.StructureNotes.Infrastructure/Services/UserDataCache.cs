using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Pi.StructureNotes.Infrastructure.Services;

public class UserDataCache : IUserDataCache
{
    private readonly DataCacheConfig _config;
    private readonly MemoryCache _userCache;

    public UserDataCache(IOptions<DataCacheConfig> options)
    {
        _config = options.Value;
        IOptions<MemoryCacheOptions> cacheOptions = Options.Create(new MemoryCacheOptions());
        _userCache = new MemoryCache(cacheOptions);
    }

    public bool TryGetUserAccounts(string userId, out IEnumerable<AccountInfo> accounts)
    {
        accounts = Enumerable.Empty<AccountInfo>();
        if (_userCache.TryGetValue(userId, out object obj))
        {
            accounts = (IEnumerable<AccountInfo>)obj;
            return true;
        }

        return false;
    }

    public void AddUserAccounts(string userId, IEnumerable<AccountInfo> accounts)
    {
        using ICacheEntry entry = _userCache.CreateEntry(userId);
        entry.Value = accounts;
        entry.AbsoluteExpiration = DateTime.UtcNow.Add(_config.Duration);
    }
}
