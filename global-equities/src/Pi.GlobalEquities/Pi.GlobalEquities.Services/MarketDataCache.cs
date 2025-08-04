
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Pi.GlobalEquities.Services.Options;

namespace Pi.GlobalEquities.Services;

public class MarketDataCache : IMarketDataCache
{
    private readonly IMemoryCache _exchangeCache;
    private readonly CacheOptions _options;

    public MarketDataCache(IMemoryCache exchangeCache, IOptions<CacheOptions> options)
    {
        _exchangeCache = exchangeCache;
        _options = options.Value;
    }

    public bool TryGetExchangeRate(Currency from, Currency to, out ExchangeRateData rate)
    {
        string key = ExchangeKey(from, to);
        rate = default;
        if (_exchangeCache.TryGetValue(key, out object obj))
        {
            rate = (ExchangeRateData)obj;
            return true;
        }
        return false;
    }

    public void AddExchangeRate(Currency from, Currency to, decimal rate, DateTime expiredAt)
    {
        var maxExpiration = DateTime.UtcNow.AddMinutes(_options.ExchangeRateExpirationMins);
        var validUntil = expiredAt < maxExpiration ? expiredAt : maxExpiration;
        var exRate = new ExchangeRateData
        {
            Rate = rate,
            ValidUntil = validUntil
        };

        var key = ExchangeKey(from, to);
        using var entry = _exchangeCache.CreateEntry(key);
        entry.Value = exRate;
        entry.AbsoluteExpiration = DateTime.UtcNow.AddDays(_options.FallbackExpirationDays);
    }

    private string ExchangeKey(Currency from, Currency to)
        => $"{from.ToString().ToUpper()}-{to.ToString().ToUpper()}";
}
