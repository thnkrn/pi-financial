namespace Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;

public interface IMemoryCacheHelper
{
    Task<string> GetCurrentSessionAsync();
    Task SetCurrentSessionAsync(string session);

    Task<ulong> GetCurrentGlimpseSequenceNoAsync();
    Task SetCurrentGlimpseSequenceNoAsync(ulong sequenceNumber);

    Task<ulong> GetCurrentItchSequenceNoAsync();
    Task SetCurrentItchSequenceNoAsync(ulong sequenceNumber);

    Task<string> GetCurrentItchLastSecondAsync();
    Task SetCurrentItchLastSecondAsync(string lastSecond);

    Task<string> GetCurrentIsNotBusinessDaysAsync(string cacheKey);

    Task SetCurrentIsNotBusinessDaysAsync(string cacheKey, string isNotBusinessDays, TimeSpan expiration);
}