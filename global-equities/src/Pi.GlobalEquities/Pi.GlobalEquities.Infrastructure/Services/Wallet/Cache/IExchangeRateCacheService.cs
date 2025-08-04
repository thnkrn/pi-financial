using Pi.Common.CommonModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Infrastructures.Services.Wallet.Cache;

public interface IExchangeRateCacheService
{
    bool TryGetExchangeRate(Currency from, Currency to, out ExchangeRateData? rate);
    void AddExchangeRate(Currency from, Currency to, decimal rate, DateTime expiredAt);
}
