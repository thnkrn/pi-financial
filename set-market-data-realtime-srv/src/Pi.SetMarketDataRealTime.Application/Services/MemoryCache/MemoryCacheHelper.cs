using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Models;

namespace Pi.SetMarketDataRealTime.Application.Services.MemoryCache;

public class MemoryCacheHelper : IMemoryCacheHelper
{
    private readonly IMemoryCacheWrapper _cache;

    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    public MemoryCacheHelper(IMemoryCacheWrapper cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<string> GetCurrentSessionAsync()
    {
        var session = await _cache.GetStringAsync(MemoryCachingKey.LoginSessionKey);

        if (!string.IsNullOrEmpty(session)) return session;

        var guid = Guid.NewGuid().ToString("N");
        await SetCurrentSessionAsync(guid);
        return guid;
    }

    public Task SetCurrentSessionAsync(string session)
    {
        return _cache.SetStringAsync(MemoryCachingKey.LoginSessionKey,
            !string.IsNullOrEmpty(session) ? session : Guid.NewGuid().ToString("N"));
    }

    public async Task<ulong> GetCurrentGlimpseSequenceNoAsync()
    {
        var seqNo = await _cache.GetStringAsync(MemoryCachingKey.GlimpseLastSequenceNoKey);
        return !string.IsNullOrEmpty(seqNo) ? ulong.Parse(seqNo) : 1;
    }

    public Task SetCurrentGlimpseSequenceNoAsync(ulong sequenceNumber)
    {
        if (sequenceNumber < 1)
            sequenceNumber = 1;

        return _cache.SetStringAsync(MemoryCachingKey.GlimpseLastSequenceNoKey, sequenceNumber.ToString());
    }

    public async Task<ulong> GetCurrentItchSequenceNoAsync()
    {
        var seqNo = await _cache.GetStringAsync(MemoryCachingKey.ItchLastSequenceNoKey);
        return !string.IsNullOrEmpty(seqNo) ? ulong.Parse(seqNo) : 1;
    }

    public Task SetCurrentItchSequenceNoAsync(ulong sequenceNumber)
    {
        if (sequenceNumber < 1)
            sequenceNumber = 1;

        return _cache.SetStringAsync(MemoryCachingKey.ItchLastSequenceNoKey, sequenceNumber.ToString());
    }

    public async Task<string> GetCurrentItchLastSecondAsync()
    {
        var lastSecond = await _cache.GetStringAsync(MemoryCachingKey.ItchLastSecondKey);
        return lastSecond ?? string.Empty;
    }

    public Task SetCurrentItchLastSecondAsync(string lastSecond)
    {
        return _cache.SetStringAsync(MemoryCachingKey.ItchLastSecondKey, lastSecond);
    }

    public async Task<string> GetCurrentIsNotBusinessDaysAsync(string cacheKey)
    {
        var isOutsideBusinessHour = await _cache.GetStringAsync($"{MemoryCachingKey.HolidayCacheKey}_{cacheKey}");
        return isOutsideBusinessHour ?? string.Empty;
    }

    public Task SetCurrentIsNotBusinessDaysAsync(string cacheKey, string isNotBusinessDays, TimeSpan expiration)
    {
        return _cache.SetStringAsync($"{MemoryCachingKey.HolidayCacheKey}_{cacheKey}", isNotBusinessDays, expiration);
    }
}