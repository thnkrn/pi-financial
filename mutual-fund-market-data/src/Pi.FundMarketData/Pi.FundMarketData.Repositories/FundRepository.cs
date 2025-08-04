using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Search;
using Pi.Common.Domain;
using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;
using Pi.FundMarketData.Models;

namespace Pi.FundMarketData.Repositories;

public class FundRepository : IFundRepository
{
    private MongoDbConfig _config;
    private IMongoClient _client;
    private ILogger<FundRepository> _logger;

    public FundRepository(IMongoClient client, IOptions<MongoDbConfig> options, ILogger<FundRepository> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }

    public async Task UpdateFund(string symbol, UpdateDefinition<Fund> patch, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentNullException(nameof(symbol));
        }

        var db = _client.GetDatabase(_config.Database);
        var col = db.GetCollection<Fund>(MongoDbConfig.FundProfilesColName);
        var filter = Builders<Fund>.Filter.Eq(x => x.Symbol, symbol.ToUpper());
        await col.UpdateOneAsync(filter,
            patch,
            new UpdateOptions { IsUpsert = false },
            ct);
    }

    public async Task UpdateFundByMorningstarId(string mstarId, UpdateDefinition<Fund> patch,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(mstarId))
        {
            throw new ArgumentNullException(nameof(mstarId));
        }

        var db = _client.GetDatabase(_config.Database);
        var col = db.GetCollection<Fund>(MongoDbConfig.FundProfilesColName);
        var filter = Builders<Fund>.Filter.Eq(x => x.MorningstarId, mstarId);

        await col.UpdateOneAsync(filter,
            patch,
            new UpdateOptions { IsUpsert = false },
            ct);
    }

    public async Task UpsertFundWithPreviousSymbol(string symbol, string previousSymbol, UpdateDefinition<Fund> patch, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentNullException(nameof(symbol));
        }

        var db = _client.GetDatabase(_config.Database);
        var col = db.GetCollection<Fund>(MongoDbConfig.FundProfilesColName);
        var filter = Builders<Fund>.Filter.Or(
                        Builders<Fund>.Filter.Eq(x => x.Symbol, symbol.ToUpper()),
                        Builders<Fund>.Filter.Eq(x => x.Symbol, previousSymbol.ToUpper()));
        try
        {
            await col.UpdateOneAsync(filter,
               patch,
               new UpdateOptions { IsUpsert = true },
               ct);
        }
        catch (MongoWriteException ex)
        {
            // To handle the cases from Fundconnext that "there is a fund that change its name to new name and there is another fund using the first fund previous name."
            if (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey && (ex.WriteError?.Message?.Contains(MongoDbConfig.SymbolIndexName) ?? false))
            {
                _logger.LogWarning("{ex.category} of Symbol = {symbol} or {previousSymbol}.", ex.WriteError?.Category, symbol, previousSymbol);
                var deleteFilter = Builders<Fund>.Filter.Eq(x => x.Symbol, previousSymbol.ToUpper());
                await col.DeleteOneAsync(deleteFilter, ct);

                filter = Builders<Fund>.Filter.Eq(x => x.Symbol, symbol.ToUpper());
                await col.UpdateOneAsync(filter,
                    patch,
                    new UpdateOptions { IsUpsert = false },
                    ct);
            }
            else
            {
                throw;
            }
        }
    }

    public async Task UpdateFundNavFromMorningStar(string symbol, DateTime? morningStarNavLatestDate, UpdateDefinition<Fund> patch, CancellationToken ct)
    {
        if (morningStarNavLatestDate == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentNullException(nameof(symbol));
        }

        var db = _client.GetDatabase(_config.Database);
        var col = db.GetCollection<Fund>(MongoDbConfig.FundProfilesColName);
        var filter = Builders<Fund>.Filter.And(Builders<Fund>.Filter.Eq(x => x.Symbol, symbol),
                        Builders<Fund>.Filter.Or(
                            Builders<Fund>.Filter.Lt(x => x.AssetValue.AsOfDate, morningStarNavLatestDate),
                            Builders<Fund>.Filter.Eq(x => x.AssetValue.AsOfDate, null)
                        )
                    );

        await col.UpdateOneAsync(filter,
            patch,
            new UpdateOptions { IsUpsert = false },
            ct);
    }

    public async Task<List<Fund>> GetFundProfiles(IList<string> symbols, CancellationToken ct)
    {
        if (symbols is null || !symbols.Any())
            return new();

        var db = _client.GetDatabase(_config.Database);
        var query = db.GetCollection<Fund>(MongoDbConfig.FundProfilesColName).AsQueryable();

        var results = await query.Where(x => symbols.Contains(x.Symbol)).ToListAsync(ct);

        return results;
    }

    public async Task<Fund> GetFundProfile(string symbol, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentNullException(nameof(symbol));
        }

        var db = _client.GetDatabase(_config.Database);
        var query = db.GetCollection<Fund>(MongoDbConfig.FundProfilesColName).AsQueryable();

        var result = await query.Where(x => x.Symbol == symbol).FirstOrDefaultAsync(ct);

        return result;
    }

    public async Task<IEnumerable<Fund>> GetFundProfilesByMarketBasket(
        MarketBasket marketBasket,
        Interval interval,
        CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var query = db.GetCollection<Fund>(MongoDbConfig.FundProfilesColName).AsQueryable();
        query = GetBasketQuery(query, marketBasket, interval);

        return marketBasket switch
        {
            MarketBasket.Category or MarketBasket.NewFund or MarketBasket.TopFund => await query.ToListAsync(ct),
            _ => (IEnumerable<Fund>)[]
        };
    }

    public async Task<PaginateResult<Fund>> PaginateBasketFundProfiles(MarketBasket marketBasket, Interval interval,
        int pageNum, int pageSize, IQueryFilter<Fund> filters = null, CancellationToken ct = default)
    {
        var db = _client.GetDatabase(_config.Database);
        var query = db.GetCollection<Fund>(MongoDbConfig.FundProfilesColName).AsQueryable();

        query = GetBasketQuery(query, marketBasket, interval);
        switch (marketBasket)
        {
            case MarketBasket.TopFund:
                var topFunds = await query.ToListAsync(ct);
                int totalTopFunds = await query.CountAsync(ct);

                return new PaginateResult<Fund>(topFunds, pageNum, pageSize, totalTopFunds);
            case MarketBasket.NewFund:
                break;
            case MarketBasket.Category:
                if (filters != null)
                {
                    var expressions = filters.GetExpressions();
                    expressions.ForEach(q =>
                    {
                        query = query.Where(q);
                    });
                }

                break;
            default:
                return new PaginateResult<Fund>([], pageNum, pageSize, 0);
        }

        var records = await query.Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        int total = await query.CountAsync(ct);

        return new PaginateResult<Fund>(records, pageNum, pageSize, total);
    }

    private static IQueryable<Fund> GetBasketQuery(IQueryable<Fund> query, MarketBasket basket, Interval interval)
    {
        switch (basket)
        {
            case MarketBasket.TopFund:
                return query.Where(x => x.Performance.HistoricalReturnPercentages != null)
                    .OrderByDescending(x => x.Performance.HistoricalReturnPercentages.FirstOrDefault(y => y.Interval == interval))
                    .Take(30);
            case MarketBasket.NewFund:
                (DateTime start, DateTime end) = Interval.Over3Months.GetIntervalDateTimes();

                return query.Where(q => q.Fundamental.InceptionDate <= end && q.Fundamental.InceptionDate >= start)
                    .OrderByDescending(q => q.Performance.HistoricalReturnPercentages.FirstOrDefault(y => y.Interval == interval));
            case MarketBasket.Category:
                return query.OrderBy(q => q.Name);
            default:
                return query;
        }
    }

    public async Task<IEnumerable<FundSearchData>> SearchFundProfiles(string keyword, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return Enumerable.Empty<FundSearchData>();

        var db = _client.GetDatabase(_config.Database);
        var col = db.GetCollection<Fund>(MongoDbConfig.FundProfilesColName);

        var searchOptions = new SearchOptions<Fund>()
        {
            Sort = Builders<Fund>.Sort.Ascending(x => x.Symbol),
            IndexName = MongoDbConfig.FundSearchIndexName
        };

        var results = await col.Aggregate()
            .Search(Builders<Fund>.Search.Compound()
                .Should(
                    Builders<Fund>.Search.Wildcard(g => g.Symbol, "*" + keyword + "*", true),
                    Builders<Fund>.Search.Wildcard(g => g.Name, "*" + keyword + "*", true)
                    ), searchOptions: searchOptions)
            .Limit(50)
            .Project(f => new FundSearchData
            {
                Symbol = f.Symbol,
                Name = f.Name,
                Nav = f.AssetValue.Nav,
                ValueChange = f.AssetValue.NavChange.ValueChange,
                NavChangePercentage = f.AssetValue.NavChange.NavChangePercentage,
                Currency = f.Fundamental.Currency,
                AmcCode = f.AmcCode
            })
            .ToListAsync(ct);

        return results;
    }

    public async Task ReplaceWhitelistSymbols(IEnumerable<string> symbols, CancellationToken ct)
    {
        if (!symbols.Any())
            throw new ArgumentNullException(nameof(symbols));

        var db = _client.GetDatabase(_config.Database);
        var col = db.GetCollection<FundWhitelist>(MongoDbConfig.FundWhitelistsColName);

        var deleteAllFilter = Builders<FundWhitelist>.Filter.Empty;
        await col.DeleteManyAsync(deleteAllFilter, ct);

        var whitelistDocs = symbols.Select(x => new FundWhitelist { Symbol = x });
        await col.InsertManyAsync(whitelistDocs, cancellationToken: ct);
    }

    public async Task<IEnumerable<string>> GetWhitelistSymbols(CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var col = db.GetCollection<FundWhitelist>(MongoDbConfig.FundWhitelistsColName);

        return await col.AsQueryable()
            .Select(x => x.Symbol)
            .ToListAsync(ct);
    }

    public async Task<bool> WhiteListSymbolExists(string symbol, CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var col = db.GetCollection<FundWhitelist>(MongoDbConfig.FundWhitelistsColName);

        return await col.AsQueryable().AnyAsync(x => x.Symbol == symbol, ct);
    }
}
