using Pi.FundMarketData.Worker.Services.FundMarket.Morningstar.Models;

namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar
{
    public interface IMorningstarService
    {
        Task Authenticate(CancellationToken ct);
        Task<IEnumerable<FundBasicInfo>> GetFundBasicInfos(CancellationToken ct);
        Task<IEnumerable<FundPerformance>> GetFundPerformances(CancellationToken ct);
        Task<IEnumerable<AssetClassAllocation>> GetAssetClassAllocations(CancellationToken ct);
        Task<IEnumerable<StockSectorAllocation>> GetStockSectorAllocations(CancellationToken ct);
        Task<IEnumerable<FeeAndExpense>> GetFeesAndExpenses(CancellationToken ct);
        Task<IEnumerable<RegionalAllocation>> GetRegionalAllocations(CancellationToken ct);
        Task<UnderlyingHolding> GetTop25UnderlyingHolding(string mstarId, CancellationToken ct);
        Task<HistoricalDistribution> GetHistoricalDistribution(string mstarId, DateTime startDate, DateTime endDate, CancellationToken ct);
        Task<HistoricalNav> GetHistoricalNav(string mstarId, DateTime startDate, DateTime endDate, CancellationToken ct);
    }
}
