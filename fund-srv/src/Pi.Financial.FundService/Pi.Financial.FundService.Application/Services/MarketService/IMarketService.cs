using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.Financial.FundService.Application.Services.MarketService;

public interface IMarketService
{
    Task<IEnumerable<FundInfo>> GetFundInfosAsync(IEnumerable<string> fundCodes, CancellationToken cancellationToken = default);
    Task<FundInfo?> GetFundInfoByFundCodeAsync(string fundCode, CancellationToken cancellationToken = default);
}
