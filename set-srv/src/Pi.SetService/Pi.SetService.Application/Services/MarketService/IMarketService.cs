using Pi.SetService.Application.Models;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Application.Services.MarketService;

public interface IMarketService
{
    Task<List<InstrumentProfile>> GetInstrumentsProfile(string[] symbols,
        CancellationToken ct = default);

    Task<List<EquityInstrument>> GetEquityInstruments(string[] symbols,
        CancellationToken ct = default);

    Task<InstrumentProfile?> GetInstrumentProfile(string symbol, CancellationToken ct = default);
    Task<TradingDetail?> GetTradingDetail(string symbol, CancellationToken ct = default);
    Task<IEnumerable<CorporateAction>> GetCorporateActions(string symbol, CancellationToken ct = default);

    Task<CeilingFloor?> GetCeilingFloor(string symbol, CancellationToken ct = default);
    Task<MarketStatus> GetCurrentMarketStatus();
}
