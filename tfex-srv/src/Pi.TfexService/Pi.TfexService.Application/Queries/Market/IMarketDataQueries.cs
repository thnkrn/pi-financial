using Pi.TfexService.Application.Models;

namespace Pi.TfexService.Application.Queries.Market;

public record MarketData(
    string Series,
    string InstrumentCategory,
    decimal? TickSize = null,
    decimal? LotSize = null,
    decimal? Multiplier = null,
    MultiplierType? MultiplierType = null,
    MultiplierUnit? MultiplierUnit = null,
    string? Logo = null
);

public interface IMarketDataQueries
{
    Task<MarketData?> GetMarketData(string? sid, string series, CancellationToken cancellationToken = default);
    Task<IDictionary<string, MarketData>> GetMarketData(string? sid, List<string> series, CancellationToken cancellationToken = default);
}