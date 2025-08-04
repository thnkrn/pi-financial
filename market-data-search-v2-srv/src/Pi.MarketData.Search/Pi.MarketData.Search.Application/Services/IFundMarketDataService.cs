using Pi.MarketData.Search.Domain.Models;

namespace Pi.MarketData.Search.Application.Services;

public interface IFundMarketDataService
{
    Task<IEnumerable<InstrumentSummary>> GetFundSummaries(IEnumerable<string> fundCodes, CancellationToken ct);
    Task<IEnumerable<InstrumentSummary>> GetTopFundsOver3Months(int limit = 30, CancellationToken ct = default);
    Task<IEnumerable<InstrumentSummary>> SearchFunds(string keyword, CancellationToken ct);
}
