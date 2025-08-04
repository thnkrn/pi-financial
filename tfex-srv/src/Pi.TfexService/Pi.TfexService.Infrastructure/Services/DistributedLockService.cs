using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Pi.TfexService.Application.Services.DistributedLock;
using Pi.TfexService.Infrastructure.Options;
using RedLockNet;

namespace Pi.TfexService.Infrastructure.Services;

public class DistributedLockService(
    IDistributedLockFactory redLockFactory,
    IDistributedCache distributedCache,
    IOptionsMonitor<FeaturesOptions> options)
    : IDistributedLockService
{
    public async Task<bool> AddEventAsync(string eventName)
    {
        var key = GetEventName(eventName);
        await using var redLock = await redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(10));
        if (!redLock.IsAcquired) return false;
        if (await distributedCache.GetAsync(key) != null) return false;

        await distributedCache.SetAsync(key, GetDefaultByteArray(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow =
                TimeSpan.FromSeconds(options.CurrentValue.TfexNotificationExpireTimeSecond)
        });
        return true;
    }

    public async Task<bool> RemoveEventAsync(string eventName)
    {
        var key = GetEventName(eventName);
        if (await distributedCache.GetAsync(key) == null) return false;

        await distributedCache.RemoveAsync(key);
        return true;
    }

    private static string GetEventName(string eventName)
    {
        return $"lock-event:{eventName}";
    }

    private static byte[] GetDefaultByteArray()
    {
        return "1"u8.ToArray();
    }
}