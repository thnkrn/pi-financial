using System.Globalization;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Repositories;

public class FundHistoricalNavRepository : IFundHistoricalNavRepository
{
    private MongoDbConfig _config;
    private IMongoClient _client;
    private readonly DataCacheConfig _cacheConfig;
    private readonly IDistributedCache _cache;
    private const string _latestNavDateKeyPrefix = "FUND_MD::LATEST_NAV_DATE";

    public FundHistoricalNavRepository(IMongoClient client, IOptions<MongoDbConfig> options, IDistributedCache cache, IOptions<DataCacheConfig> cacheOptions)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
        _cache = cache;
        _cacheConfig = cacheOptions.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task BulkReplaceHistoricalNavs(string symbol, IEnumerable<HistoricalNav> navs, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentNullException(nameof(symbol));

        var db = _client.GetDatabase(_config.Database);
        var col = db.GetCollection<HistoricalNav>(MongoDbConfig.HistoricalNavsColName);

        var filter = Builders<HistoricalNav>.Filter.Eq(x => x.Symbol, symbol.ToUpper());

        await col.DeleteManyAsync(filter, cancellationToken: ct);
        await ProcessInBatchesAsync(navs, async batch =>
        {
            await col.InsertManyAsync(batch, new InsertManyOptions { IsOrdered = false }, ct);
        });
    }

    public async Task<HistoricalNavInfo> GetHistoricalNavInfo(string symbol, Interval interval, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentNullException(nameof(symbol));

        var db = _client.GetDatabase(_config.Database);
        var query = db.GetCollection<HistoricalNav>(MongoDbConfig.HistoricalNavsColName).AsQueryable();

        DateTime latestDateNav;
        var cachedValue = await _cache.GetAsync(GetLatestNavDate(symbol), ct);
        if (cachedValue != null)
        {
            var cacheString = Encoding.UTF8.GetString(cachedValue);
            DateTime.TryParse(cacheString, CultureInfo.InvariantCulture, out latestDateNav);
        }
        else
        {
            latestDateNav = await query.Where(his => his.Symbol == symbol).Select(mHis => mHis.Date)
                .OrderByDescending(x => x.Date).FirstOrDefaultAsync(ct);

            if (latestDateNav != default)
            {
                var cacheOptions =
                    new DistributedCacheEntryOptions().SetAbsoluteExpiration(_cacheConfig.LatestNavCacheDuration);

                await _cache.SetAsync(GetLatestNavDate(symbol),
                    Encoding.UTF8.GetBytes(latestDateNav.ToString(CultureInfo.InvariantCulture)), cacheOptions, ct);
            }
            else
                return null;
        }

        (DateTime start, DateTime end) = interval.GetIntervalDateTimes(latestDateNav);
        var navs = await query.Where(x => x.Symbol == symbol && x.Date >= start && x.Date <= end).ToListAsync(ct);

        return navs.Count != 0
            ? new HistoricalNavInfo(navs, interval)
            : null;
    }

    public async Task<List<HistoricalNavInfo>> GetHistoricalNavInfos(
        string[] symbols,
        Interval interval,
        CancellationToken ct)
    {
        if (symbols == null || symbols.Length == 0)
            throw new ArgumentNullException(nameof(symbols));

        string[] distinctSymbols = symbols
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var db = _client.GetDatabase(_config.Database);
        var navCollection = db.GetCollection<HistoricalNav>(MongoDbConfig.HistoricalNavsColName);

        var navInfos = new List<HistoricalNavInfo>();
        var latestNavDateMap = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        var symbolsToQueryDb = new List<string>();

        foreach (string symbol in distinctSymbols)
        {
            var cachedDate = await GetCacheLatestNavDateAsync(symbol, ct);

            if (cachedDate != null)
            {
                latestNavDateMap[symbol] = cachedDate.Value;
            }
            else
            {
                symbolsToQueryDb.Add(symbol);
            }
        }

        if (symbolsToQueryDb.Count > 0)
        {
            var latestDates = await navCollection
                .AsQueryable()
                .Where(h => symbolsToQueryDb.Contains(h.Symbol))
                .GroupBy(h => h.Symbol)
                .Select(g => new { Symbol = g.Key, LatestDate = g.Max(x => x.Date) })
                .ToListAsync(ct);

            foreach (var entry in latestDates.Where(entry => entry.LatestDate != default))
            {
                latestNavDateMap[entry.Symbol] = entry.LatestDate;
                await SetCacheLatestNavDateAsync(entry.Symbol, entry.LatestDate, ct);
            }
        }

        foreach ((string symbol, DateTime latestDateNav) in latestNavDateMap)
        {
            (DateTime start, DateTime end) = interval.GetIntervalDateTimes(latestDateNav);

            var navs = await navCollection
                .AsQueryable()
                .Where(n => n.Symbol == symbol && n.Date >= start && n.Date <= end)
                .GroupBy(n => n.Date)
                .Select(g => g.First())
                .ToListAsync(ct);

            if (navs.Count > 0)
            {
                navInfos.Add(new HistoricalNavInfo(navs, interval));
            }
        }

        return navInfos;
    }

    private async Task<DateTime?> GetCacheLatestNavDateAsync(string symbol, CancellationToken ct)
    {
        string cacheKey = GetLatestNavDate(symbol);
        byte[] cachedValue = await _cache.GetAsync(cacheKey, ct);

        if (cachedValue == null)
        {
            return null;
        }

        string cacheString = Encoding.UTF8.GetString(cachedValue);
        if (DateTime.TryParse(cacheString, CultureInfo.InvariantCulture, out var cachedDate))
        {
            return cachedDate;
        }

        return null;
    }

    private async Task SetCacheLatestNavDateAsync(string symbol, DateTime latestDate, CancellationToken ct)
    {
        string cacheKey = GetLatestNavDate(symbol);
        string dateString = latestDate.ToString(CultureInfo.InvariantCulture);
        byte[] bytes = Encoding.UTF8.GetBytes(dateString);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheConfig.LatestNavCacheDuration
        };

        await _cache.SetAsync(cacheKey, bytes, cacheOptions, ct);
    }

    private static async Task ProcessInBatchesAsync<T>(
        IEnumerable<T> documents,
        Func<List<T>, Task> action,
        int batchSize = 1000
    )
    {
        var documentList = documents.ToList();
        int total = documentList.Count;

        for (int i = 0; i < total; i += batchSize)
        {
            var batch = documentList.GetRange(i, Math.Min(batchSize, total - i));

            await action(batch);
        }
    }

    private static string GetLatestNavDate(string symbol)
    {
        return $"{_latestNavDateKeyPrefix}_{symbol}";
    }
}
