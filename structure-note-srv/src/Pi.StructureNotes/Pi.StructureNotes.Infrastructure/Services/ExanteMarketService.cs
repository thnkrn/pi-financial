using System.Net.Http.Json;

namespace Pi.StructureNotes.Infrastructure.Services;

public class ExanteMarketService : IMarketService
{
    private readonly IMarketDataCache _cache;
    private readonly HttpClient _client;

    public ExanteMarketService(HttpClient client, IMarketDataCache cache = null)
    {
        _client = client;
        _cache = cache;
    }

    public async Task<IExchangeRateLookup> GetExchangeLookup(IEnumerable<string> fromCurrencies,
        string to, CancellationToken ct)
    {
        List<Task<CurrencyExchange>> tasks = new List<Task<CurrencyExchange>>();
        foreach (string from in fromCurrencies)
        {
            if (from != to)
            {
                tasks.Add(GetExchangeRate(from, to, ct));
            }
        }

        await Task.WhenAll(tasks);

        IEnumerable<CurrencyExchange> results = tasks.Where(x => x.IsCompletedSuccessfully).Select(x => x.Result);

        ExchangeLookup lookup = new ExchangeLookup();
        foreach (CurrencyExchange result in results)
        {
            lookup.Add(result);
        }

        return lookup;
    }

    public async Task<StockPrice> GetStockPrice(string symbol, string currency, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException(nameof(symbol));
        }

        string symbolCurrency = await GetSymbolCurrency(symbol, ct);

        decimal localPrice = await GetSockLastClosePrice(symbol, ct);
        decimal price = localPrice;
        if (symbolCurrency != currency)
        {
            CurrencyExchange exchange = await GetExchangeRate(symbolCurrency, currency, ct);
            price = price * exchange.Rate;
        }

        return new StockPrice(currency, price);
    }

    private async Task<decimal> GetSockLastClosePrice(string symbol, CancellationToken ct)
    {
        string url = $"/md/3.0/change/{symbol}";
        var dChanges = await _client.GetFromJsonAsync<DailyChange[]>(url, ct);
        var dChange = dChanges.FirstOrDefault();

        return dChange?.lastSessionClosePrice ?? 0;
    }

    private async Task<CurrencyExchange> GetExchangeRate(string from, string to, CancellationToken ct)
    {
        from = from.ToUpper();
        to = to.ToUpper();
        if (from == to)
        {
            return new CurrencyExchange(from, to, 1);
        }

        if (!_cache.TryGetExchangeRate(from, to, out decimal rate))
        {
            string url = $"/md/3.0/crossrates/{from}/{to}";
            ExRate result = await _client.GetFromJsonAsync<ExRate>(url, ct);
            rate = result.rate;
            _cache.AddExchangeRate(from, to, rate);
        }

        return new CurrencyExchange(from, to, rate);
    }

    private async Task<string> GetSymbolCurrency(string symbol, CancellationToken ct)
    {
        if (!_cache.TryGetSymbolCurrency(symbol, out string currency))
        {
            string url = $"/md/3.0/symbols/{symbol}";
            Symbol symbolData = await _client.GetFromJsonAsync<Symbol>(url, ct);

            if (symbolData == null)
            {
                throw new Exception($"Exante return null for symbole data: /md/3.0/symbols/{symbol}");
            }

            if (string.IsNullOrWhiteSpace(symbolData.currency))
            {
                throw new Exception($"Exante return null or empty curreny: /md/3.0/symbols/{symbol}");
            }

            currency = symbolData.currency;
            _cache.AddSymbolCurrency(symbol, currency);
        }

        return currency.ToUpper();
    }

    private record DailyChange(decimal lastSessionClosePrice);

    private record ExRate(decimal rate);

    private record Symbol(string currency);
}
