using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pi.Common.Domain;
using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;
using Pi.FundMarketData.Models;
using Pi.FundMarketData.Utils;

namespace Pi.FundMarketData.Repositories;

public class FundWebRepository(IMongoClient client, IOptions<MongoDbConfig> options) : IFundWebRepository
{
    private readonly MongoDbConfig _config = options.Value;
    private readonly IMongoClient _client = client ?? throw new ArgumentNullException(nameof(client));

    public async Task<List<Fund>> GetFundMarketSummaries(string[] symbols,
        CancellationToken cancellationToken = default)
    {
        if (symbols == null || symbols.Length == 0)
        {
            throw new ArgumentNullException(nameof(symbols));
        }

        var distinctSymbols = symbols
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => UtilsMethod.ReplaceEncodedSlash(s).Trim().ToUpperInvariant())
            .Distinct()
            .ToList();

        var database = _client.GetDatabase(_config.Database);
        var collection = database.GetCollection<Fund>(MongoDbConfig.FundProfilesColName);

        var result = await collection
            .AsQueryable()
            .Where(fund => distinctSymbols.Contains(fund.Symbol.ToUpper()))
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<PaginateResult<Fund>> GetPaginateFundMarketSummaries(
        MarketBasket marketBasket,
        Interval interval,
        int pageNum,
        int pageSize,
        IQueryFilter<Fund> filters = null,
        CancellationToken cancellationToken = default)
    {
        var database = _client.GetDatabase(_config.Database);
        var collection = database.GetCollection<Fund>(MongoDbConfig.FundProfilesColName);
        var query = collection.AsQueryable();

        query = ApplyBasketFilter(query, marketBasket, interval);

        if (filters is not null)
        {
            query = filters.GetExpressions().Aggregate(query, (current, expression) => current.Where(expression));
        }

        int skip = (pageNum - 1) * pageSize;
        var records = await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);
        int total = await query.CountAsync(cancellationToken);

        return new PaginateResult<Fund>(records, pageNum, pageSize, total);
    }

    private static IQueryable<Fund> ApplyBasketFilter(IQueryable<Fund> query, MarketBasket basket, Interval interval)
    {
        return basket switch
        {
            MarketBasket.TopFund => query
                .Where(f => f.Performance.HistoricalReturnPercentages != null)
                .OrderByDescending(f =>
                    f.Performance.HistoricalReturnPercentages
                        .FirstOrDefault(p => p.Interval == interval)),
            MarketBasket.NewFund => GetNewFundQuery(query, interval),
            MarketBasket.Category => query.OrderBy(f => f.Name),
            _ => query.OrderBy(f => f.Id)
        };
    }

    private static IQueryable<Fund> GetNewFundQuery(IQueryable<Fund> query, Interval interval)
    {
        var (start, end) = Interval.Over3Months.GetIntervalDateTimes();

        return query
            .Where(f => f.Fundamental.InceptionDate >= start && f.Fundamental.InceptionDate <= end)
            .OrderByDescending(f =>
                f.Performance.HistoricalReturnPercentages
                    .FirstOrDefault(p => p.Interval == interval));
    }

    public async Task<Dictionary<string, bool>> GetWhiteListSymbolsExists(IEnumerable<string> symbols, CancellationToken cancellationToken = default)
    {
        var db = _client.GetDatabase(_config.Database);
        var collection = db.GetCollection<FundWhitelist>(MongoDbConfig.FundWhitelistsColName);

        var distinctSymbols = symbols
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => UtilsMethod.ReplaceEncodedSlash(s).Trim().ToUpperInvariant())
            .Distinct()
            .ToList();

        var result = await collection
            .AsQueryable()
            .Where(fund => distinctSymbols.Contains(fund.Symbol.ToUpper()))
            .ToListAsync(cancellationToken);

        return distinctSymbols
            .ToDictionary(symbol => symbol,
                symbol => result.Any(fund => fund.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase)));
    }

}
