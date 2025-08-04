using MongoDB.Driver;
using Pi.Common.Domain;
using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;
using Pi.FundMarketData.Models;

namespace Pi.FundMarketData.Repositories;

public interface IFundRepository
{
    Task UpdateFund(string symbol, UpdateDefinition<Fund> patch, CancellationToken ct);
    Task UpdateFundByMorningstarId(string mstarId, UpdateDefinition<Fund> patch, CancellationToken ct);

    Task UpsertFundWithPreviousSymbol(string symbol, string previousSymbol, UpdateDefinition<Fund> patch,
        CancellationToken ct);

    Task UpdateFundNavFromMorningStar(string symbol, DateTime? morningStarNavLatestDate, UpdateDefinition<Fund> patch, CancellationToken ct);

    Task<IEnumerable<Fund>> GetFundProfilesByMarketBasket(MarketBasket marketBasket, Interval interval,
        CancellationToken ct);

    Task<PaginateResult<Fund>> PaginateBasketFundProfiles(MarketBasket marketBasket, Interval interval, int pageNum,
        int pageSize, IQueryFilter<Fund> filters = null, CancellationToken ct = default);
    Task<Fund> GetFundProfile(string symbol, CancellationToken ct);
    Task<List<Fund>> GetFundProfiles(IList<string> symbols, CancellationToken ct);
    Task<IEnumerable<FundSearchData>> SearchFundProfiles(string keyword, CancellationToken ct);

    Task ReplaceWhitelistSymbols(IEnumerable<string> symbols, CancellationToken ct);
    Task<IEnumerable<string>> GetWhitelistSymbols(CancellationToken ct);
    Task<bool> WhiteListSymbolExists(string symbol, CancellationToken ct);
}
