namespace Pi.StructureNotes.Infrastructure.Services;

public interface IMarketService
{
    Task<StockPrice> GetStockPrice(string symbol, string currency, CancellationToken ct = default);

    Task<IExchangeRateLookup> GetExchangeLookup(IEnumerable<string> fromCurrencies, string targetCurrency,
        CancellationToken ct = default);
}
