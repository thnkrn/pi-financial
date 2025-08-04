namespace Pi.GlobalEquities.Services;

public interface IMarketDataCache
{
    bool TryGetExchangeRate(Currency from, Currency to, out ExchangeRateData rate);
    void AddExchangeRate(Currency from, Currency to, decimal rate, DateTime expiredAt);
}
