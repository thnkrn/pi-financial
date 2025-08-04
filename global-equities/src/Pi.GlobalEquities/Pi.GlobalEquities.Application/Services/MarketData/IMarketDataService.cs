using Pi.GlobalEquities.DomainModels;

namespace Pi.GlobalEquities.Application.Services.MarketData;

public interface IMarketDataService
{
    Task<IEnumerable<SymbolPrice>> GetTicker(IEnumerable<string> symbols, CancellationToken ct);
}
