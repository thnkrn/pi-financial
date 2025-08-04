using Microsoft.Extensions.Caching.Memory;
using Pi.Common.CommonModels;
using Pi.GlobalEquities.Infrastructures.Services.Wallet.Cache;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Infrastructure.Services.Wallet.Cache;

public class ExchangeRateCacheService : IExchangeRateCacheService
{
    private readonly IMemoryCache _exchangeCache;

    public ExchangeRateCacheService(IMemoryCache exchangeCache)
    {
        _exchangeCache = exchangeCache;
    }

    public bool TryGetExchangeRate(Currency from, Currency to, out ExchangeRateData? rate)
    {
        string key = ExchangeKey(from, to);
        rate = null;
        if (_exchangeCache.TryGetValue(key, out ExchangeRateData? obj))
        {
            rate = obj;
            return true;
        }
        return false;
    }

    public void AddExchangeRate(Currency from, Currency to, decimal rate, DateTime expiredAt)
    {
        var maxExpiration = DateTime.UtcNow.AddMinutes(5);
        var validUntil = expiredAt < maxExpiration ? expiredAt : maxExpiration;
        var exRate = new ExchangeRateData
        {
            Rate = rate,
            ValidUntil = validUntil
        };

        var key = ExchangeKey(from, to);
        using var entry = _exchangeCache.CreateEntry(key);
        entry.Value = exRate;
        entry.AbsoluteExpiration = DateTime.UtcNow.AddDays(7);
    }

    private string ExchangeKey(Currency from, Currency to)
        => $"{from.ToString().ToUpper()}-{to.ToString().ToUpper()}";
}
