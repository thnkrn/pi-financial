using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Pi.StructureNotes.Infrastructure.Services;

public class MarketDataCache : IMarketDataCache
{
    private readonly DataCacheConfig _config;
    private readonly MemoryCache _exchangeCache;
    private readonly MemoryCache _symbolCache;

    public MarketDataCache(IOptions<DataCacheConfig> options)
    {
        _config = options.Value;
        IOptions<MemoryCacheOptions> cacheOptions = Options.Create(new MemoryCacheOptions());
        _symbolCache = new MemoryCache(cacheOptions);
        _exchangeCache = new MemoryCache(cacheOptions);
    }

    public bool TryGetSymbolCurrency(string symbol, out string currency)
    {
        string key = SymbolKey(symbol);
        currency = default;
        if (_symbolCache.TryGetValue(key, out object obj))
        {
            currency = (string)obj;
            return true;
        }

        return false;
    }

    public bool TryGetExchangeRate(string from, string to, out decimal rate)
    {
        string key = ExchangeKey(from, to);
        rate = default;
        if (_exchangeCache.TryGetValue(key, out object obj))
        {
            rate = (decimal)obj;
            return true;
        }

        return false;
    }

    public void AddSymbolCurrency(string symbol, string currency)
    {
        string key = SymbolKey(symbol);
        using ICacheEntry entry = _symbolCache.CreateEntry(key);
        entry.Value = currency;
        entry.AbsoluteExpiration = DateTime.UtcNow.Add(_config.Duration);
    }

    public void AddExchangeRate(string from, string to, decimal rate)
    {
        string key = ExchangeKey(from, to);
        using ICacheEntry entry = _exchangeCache.CreateEntry(key);
        entry.Value = rate;
        entry.AbsoluteExpiration = DateTime.UtcNow.Add(_config.Duration);
    }

    private string SymbolKey(string symbol) => symbol.ToUpper();

    private string ExchangeKey(string from, string to) => $"{from.ToUpper()}-{to.ToUpper()}";
}
