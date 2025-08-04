using MongoDB.Driver;
using Pi.Common.Domain;
using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;
using Pi.FundMarketData.Models;

namespace Pi.FundMarketData.Repositories;

public interface IFundWebRepository
{
    Task<List<Fund>> GetFundMarketSummaries(string[] symbols, CancellationToken ct = default);
    Task<PaginateResult<Fund>> GetPaginateFundMarketSummaries(MarketBasket marketBasket, Interval interval, int pageNum,
        int pageSize, IQueryFilter<Fund> filters = null, CancellationToken ct = default);
    Task<Dictionary<string, bool>> GetWhiteListSymbolsExists(IEnumerable<string> symbols,
        CancellationToken cancellationToken = default);
}
