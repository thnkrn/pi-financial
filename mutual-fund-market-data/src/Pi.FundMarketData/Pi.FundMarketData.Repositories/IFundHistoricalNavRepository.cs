using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Repositories;

public interface IFundHistoricalNavRepository
{
    Task BulkReplaceHistoricalNavs(string symbol, IEnumerable<HistoricalNav> navs, CancellationToken ct);
    Task<HistoricalNavInfo> GetHistoricalNavInfo(string symbol, Interval interval, CancellationToken ct);
    Task<List<HistoricalNavInfo>> GetHistoricalNavInfos(string[] symbols, Interval interval, CancellationToken ct);
}
