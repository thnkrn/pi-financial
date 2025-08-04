using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketData.Application.Helpers;
using Pi.GlobalMarketData.Application.Services.HomeInstrument;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketFilterInstruments;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.GlobalMarketData.Infrastructure.Services.EntityCacheService;

public class EntityCacheService : IEntityCacheService
{
    private readonly ICacheService _cacheService;
    private readonly TimeSpan _cacheType1;
    private readonly TimeSpan _cacheType2;
    private readonly TimeSpan _cacheType3;
    private readonly TimeSpan _cacheType4;
    private readonly IMongoService<CuratedMember> _curatedMemberService;
    private readonly IMongoService<GeInstrument> _geInstrumentService;
    private readonly IMongoService<GeVenueMapping> _geVenueMappingService;
    private readonly ILogger<EntityCacheService> _logger;
    private readonly IMongoService<MarketSessionStatus> _marketSessionStatusService;
    private readonly IMongoService<MorningStarEtfs> _morningStarEtfsService;
    private readonly IMongoService<MorningStarStocks> _morningStarStocksService;
    private readonly IMongoService<WhiteList> _whiteListService;
    private readonly IMongoService<MarketSchedule> _marketScheduleService;
    private HashSet<string> _whiteLists;
    private List<RankingItem> _rankingItems;

    [SuppressMessage("SonarQube", "S107")]
    public EntityCacheService(IServiceProvider serviceProvider)
    {
        _cacheService = serviceProvider.GetRequiredService<ICacheService>();
        _geInstrumentService = serviceProvider.GetRequiredService<IMongoService<GeInstrument>>();
        _geVenueMappingService = serviceProvider.GetRequiredService<IMongoService<GeVenueMapping>>();
        _curatedMemberService = serviceProvider.GetRequiredService<IMongoService<CuratedMember>>();
        _morningStarStocksService = serviceProvider.GetRequiredService<IMongoService<MorningStarStocks>>();
        _morningStarEtfsService = serviceProvider.GetRequiredService<IMongoService<MorningStarEtfs>>();
        _whiteListService = serviceProvider.GetRequiredService<IMongoService<WhiteList>>();
        _marketSessionStatusService = serviceProvider.GetRequiredService<IMongoService<MarketSessionStatus>>();
        _marketScheduleService = serviceProvider.GetRequiredService<IMongoService<MarketSchedule>>();
        _logger = serviceProvider.GetRequiredService<ILogger<EntityCacheService>>();
        _whiteLists = [];

        var configuration = ConfigurationHelper.GetConfiguration();
        var type1 = configuration.GetValue<int?>(ConfigurationKeys.EntityCacheOption1) ?? 1;
        var type2 = configuration.GetValue<int?>(ConfigurationKeys.EntityCacheOption2) ?? 1;
        var type3 = configuration.GetValue<int?>(ConfigurationKeys.EntityCacheOption3) ?? 1;
        var type4 = configuration.GetValue<int?>(ConfigurationKeys.EntityCacheOption4) ?? 1;

        _cacheType1 = new TimeSpan(0, 0, type1, 0); // 1 minute
        _cacheType2 = new TimeSpan(0, type2, 0, 0); // 1 hour
        _cacheType3 = new TimeSpan(0, type3, 0, 0); // 1 hour
        _cacheType4 = new TimeSpan(type4, 0, 0, 0); // 1 day

        UpdateWhiteList().GetAwaiter().GetResult();
        _rankingItems = _cacheService.GetAsync<List<RankingItem>>($"{CacheKey.RankingItem}:GE").GetAwaiter().GetResult() ?? [];
    }

    public async Task UpdateRankingItem(List<RankingItem> rankingItems)
    {
        _rankingItems = rankingItems;
        await _cacheService.SetAsync($"{CacheKey.RankingItem}:GE", _rankingItems);
    }

    public async Task UpdateWhiteList()
    {
        var whiteList = await _whiteListService.GetAllByFilterAsync(target => target.IsWhitelist);
        _whiteLists = whiteList.Select(target => $"{target.Symbol}:{target.Exchange}").ToHashSet();
    }

    public async Task<GeVenueMapping?> GetGeVenueMappingByVenue(string venue)
    {
        var response = await _cacheService.GetAsync<GeVenueMapping>($"{CacheKey.GeVenueMapping}{venue}");

        if (response == null)
        {
            var geVenueMapping = await _geVenueMappingService.GetByFilterAsync(target =>
                target.VenueCode == venue
            );
            if (geVenueMapping != null)
                await _cacheService.SetAsync(
                    $"{CacheKey.GeVenueMapping}{venue}",
                    geVenueMapping,
                    _cacheType4
                );
            return geVenueMapping;
        }

        return response;
    }

    public async Task<GeVenueMapping?> GetGeVenueMappingByExchangeIdMs(string exchangeIdMs)
    {
        var cacheKey = $"{CacheKey.GeVenueMapping}:ExchangeIdMs:{exchangeIdMs}";
        var response = await _cacheService.GetAsync<GeVenueMapping>(cacheKey);

        if (response == null)
        {
            var geVenueMapping = await _geVenueMappingService.GetByFilterAsync(target =>
                target.ExchangeIdMs.Equals(exchangeIdMs, StringComparison.OrdinalIgnoreCase)
            );
            if (geVenueMapping != null)
                await _cacheService.SetAsync(
                    cacheKey,
                    geVenueMapping,
                    _cacheType4
                );
            return geVenueMapping;
        }

        return response;
    }

    public async Task<IEnumerable<GeInstrument>> PreCacheGeInstrument(CuratedList curatedList, TimeSpan cacheTime)
    {
        return await PreCacheSortedGeInstrument(curatedList, cacheTime);
    }

    public async Task<IEnumerable<GeInstrument>> PreCacheGeInstrument(CuratedFilter curatedFilter, TimeSpan cacheTime)
    {
        return await PreCacheSortedGeInstrument(curatedFilter, cacheTime);
    }

    public async Task<IEnumerable<GeInstrument>> PreCacheSortedGeInstrument(CuratedList curatedList, TimeSpan cacheTime)
    {
        var hash = CacheKeyHelper.GenerateCacheKey(curatedList);
        var cacheKey = $"{CacheKey.GeInstrument}:{hash}";
        var geInstruments = new List<GeInstrument>();

        _logger.LogDebug("Pre caching CuratedList: {CacheKey}", cacheKey);

        var curatedMembers = await _curatedMemberService.GetAllByFilterAsync(target =>
            target.CuratedListId == curatedList.CuratedListId
        );

        var symbols = curatedMembers.Select(member => member.Symbol).ToHashSet();
        var instrumentsMembers = await _geInstrumentService.GetAllByFilterAsync(target =>
            symbols.Contains(target.Symbol)
        );
        geInstruments.AddRange(instrumentsMembers);

        var filterExpression = HomeInstrumentService.GetGeInstrumentFilter(curatedList);
        var curatedListInstrument = await _geInstrumentService.GetAllByFilterAsync(filterExpression);
        geInstruments.AddRange(curatedListInstrument);

        // Filter with whiteList
        geInstruments = geInstruments.Where(target => _whiteLists.Contains($"{target.Symbol}:{target.Exchange}"))
            .ToList();
        geInstruments = SortGeInstrument(geInstruments, curatedMembers.ToList());

        if (geInstruments.Count > 0)
            await _cacheService.SetAsync(cacheKey, geInstruments, cacheTime);

        return geInstruments;
    }

    public async Task<IEnumerable<GeInstrument>> PreCacheSortedGeInstrument(CuratedFilter curatedFilter, TimeSpan cacheTime)
    {
        var hash = CacheKeyHelper.GenerateCacheKey(curatedFilter);
        var cacheKey = $"{CacheKey.GeInstrument}:{hash}";
        var geInstruments = new List<GeInstrument>();

        _logger.LogDebug("Pre caching CuratedFilter: {CacheKey}", cacheKey);

        var curatedMembers = await _curatedMemberService.GetAllByFilterAsync(x =>
            x.CuratedListId == curatedFilter.CuratedListId
        );
        var symbols = curatedMembers.Select(member => member.Symbol).ToHashSet();
        var curatedMemberGeInstrument = await _geInstrumentService.GetAllByFilterAsync(target =>
            symbols.Contains(target.Symbol)
        );
        geInstruments.AddRange(curatedMemberGeInstrument);

        var filterExpression = MarketFilterInstrumentsService.GetTypeCategory(curatedFilter);
        var instruments = await _geInstrumentService.GetAllByFilterAsync(filterExpression);
        geInstruments.AddRange(instruments);

        // Filter with whiteList
        geInstruments = geInstruments.Where(target => _whiteLists.Contains($"{target.Symbol}:{target.Exchange}"))
            .ToList();
        geInstruments = SortGeInstrument(geInstruments, curatedMembers.ToList());

        if (geInstruments.Count > 0)
            await _cacheService.SetAsync(cacheKey, geInstruments, cacheTime);

        return geInstruments;
    }

    public async Task<IEnumerable<GeInstrument>> GetGeInstruments(CuratedList curatedList)
    {
        var hash = CacheKeyHelper.GenerateCacheKey(curatedList);
        var cacheKey = $"{CacheKey.GeInstrument}:{hash}";
        var geInstruments = await _cacheService.GetAsync<IEnumerable<GeInstrument>>(cacheKey);
        geInstruments ??= await PreCacheGeInstrument(curatedList, new TimeSpan(0, 8, 0, 0));
        return geInstruments;
    }

    public async Task<IEnumerable<GeInstrument>> GetGeInstruments(CuratedFilter curatedFilter)
    {
        var hash = CacheKeyHelper.GenerateCacheKey(curatedFilter);
        var cacheKey = $"{CacheKey.GeInstrument}:{hash}";
        var geInstruments = await _cacheService.GetAsync<IEnumerable<GeInstrument>>(cacheKey);
        geInstruments ??= await PreCacheGeInstrument(curatedFilter, new TimeSpan(0, 8, 0, 0));
        return geInstruments;
    }

    public async Task<MorningStarEtfs?> GetMorningStarEtfs(string symbol, string exchangeId)
    {
        var response =
            await _cacheService.GetAsync<MorningStarEtfs>($"{CacheKey.MorningStarEtfs}{symbol}-{exchangeId}");

        if (response == null)
        {
            var morningStarEtfs = await _morningStarEtfsService.GetByFilterAsync(target =>
                target.Symbol == symbol && target.ExchangeId == exchangeId
            );
            if (morningStarEtfs != null)
                await _cacheService.SetAsync(
                    $"{CacheKey.MorningStarEtfs}{symbol}-{exchangeId}",
                    morningStarEtfs,
                    _cacheType2
                );
            return morningStarEtfs;
        }

        return response;
    }

    public async Task<MorningStarStocks?> GetMorningStarStocks(string symbol, string exchangeId)
    {
        var response =
            await _cacheService.GetAsync<MorningStarStocks>($"{CacheKey.MorningStarStocks}{symbol}-{exchangeId}");

        if (response == null)
        {
            var morningStarStocks = await _morningStarStocksService.GetByFilterAsync(target =>
                target.Symbol == symbol && target.ExchangeId == exchangeId
            );
            if (morningStarStocks != null)
                await _cacheService.SetAsync(
                    $"{CacheKey.MorningStarStocks}{symbol}-{exchangeId}",
                    morningStarStocks,
                    _cacheType2
                );
            return morningStarStocks;
        }

        return response;
    }

    public async Task<WhiteList?> GetWhiteList(string symbol, string exchange)
    {
        var response = await _cacheService.GetAsync<WhiteList>($"{CacheKey.WhiteList}{symbol}-{exchange}");

        if (response == null)
        {
            var whiteList = await _whiteListService.GetByFilterAsync(target =>
                (target.Exchange ?? string.Empty).Equals(exchange, StringComparison.CurrentCultureIgnoreCase) &&
                (target.Symbol ?? string.Empty).Equals(symbol, StringComparison.CurrentCultureIgnoreCase)
            );
            if (whiteList != null)
                await _cacheService.SetAsync(
                    $"{CacheKey.WhiteList}{symbol}-{exchange}",
                    whiteList,
                    _cacheType3
                );
            return whiteList;
        }

        return response;
    }

    public async Task<WhiteList?> GetWhiteListByMarket(string market)
    {
        var cacheKey = $"{CacheKey.WhiteList}:{market}";
        var response = await _cacheService.GetAsync<WhiteList>(cacheKey);

        if (response == null)
        {
            var whiteList = await _whiteListService.GetByFilterAsync(target =>
                (target.Exchange ?? string.Empty).Equals(market, StringComparison.CurrentCultureIgnoreCase) ||
                (target.Symbol ?? string.Empty).Equals(market, StringComparison.CurrentCultureIgnoreCase)
            );
            if (whiteList != null)
                await _cacheService.SetAsync(
                    cacheKey,
                    whiteList,
                    _cacheType3
                );
            return whiteList;
        }

        return response;
    }

    public async Task<MarketSchedule> GetMarketSchedule(
        string? symbol, 
        string exchange,
        string marketSession,
        DateTime currentTime
    )
    {
        var cacheKey = $"{CacheKey.MarketSchedule}{exchange}:{marketSession}:{currentTime:dd/MM/yyyy}";
        var response = await _cacheService.GetAsync<MarketSchedule>(cacheKey);

        if (response == null)
        {
            var marketScheduleList = await _marketScheduleService.GetAllByFilterAsync(target =>
                (target.Symbol.Equals(symbol) || string.IsNullOrEmpty(symbol))
                && target.Exchange.Equals(exchange)
                && (target.MarketSession ?? "").Equals(marketSession)
                && target.UTCStartTime <= currentTime
            );
            var marketSchedule = marketScheduleList.OrderByDescending(target => target.UTCStartTime).FirstOrDefault();
            if (marketSchedule != null)
                await _cacheService.SetAsync(
                    cacheKey,
                    marketSchedule,
                    new TimeSpan(0, 0, 1, 0)
                );
            return marketSchedule;
        }

        return response;
    }

    public async Task<MarketSessionStatus?> GetMarketSessionStatus(string exchange, DateTime startTime)
    {
        var cacheKey = $"{CacheKey.MarketSessionStatus}{exchange}:{startTime:HH:mm:ss}";
        var response = await _cacheService.GetAsync<MarketSessionStatus>(cacheKey);

        if (response == null)
        {
            var marketSessionStatus = await _marketSessionStatusService.GetByFilterAsync(target =>
                (target.Exchange ?? string.Empty).Equals(exchange, StringComparison.CurrentCultureIgnoreCase)
                && startTime >= target.UTCStartTime
                && startTime <= target.UTCEndTime
            );
            if (marketSessionStatus != null)
                await _cacheService.SetAsync(
                    cacheKey,
                    marketSessionStatus,
                    _cacheType1
                );
            return marketSessionStatus;
        }

        return response;
    }

    private List<GeInstrument> SortGeInstrument
    (
        List<GeInstrument> geInstruments,
        List<CuratedMember> curatedMembers
    )
    {
        // remove duplicate instrument
        var uniqueSymbols = new HashSet<string>();
        geInstruments = geInstruments
            .Where(i => i.Symbol != null && uniqueSymbols.Add(i.Symbol))
            .ToList();

        // Remove CuratedMember without Ordering
        curatedMembers = curatedMembers.Where(member =>
            member.Ordering.HasValue
            && member.Ordering.Value != 0
        ).ToList();

        // First apply ranking if rankingItems exist and no curated members
        if (_rankingItems != null && curatedMembers.Count == 0)
        {
            geInstruments = RankingHelper.Rank(_rankingItems, geInstruments);
            return geInstruments;
        }

        // Sort by CuratedMember Ordering if curated members exist
        if (curatedMembers.Count > 0)
        {
            var orderingBySymbol = curatedMembers
                .Where(
                    member => member is { Symbol: not null, Ordering: not null }
                              && member.Ordering.Value != 0
                )
                .ToDictionary(
                    member => member.Symbol!,
                    member => member.Ordering!.Value
                );

            geInstruments = geInstruments
                .Where(instrument => !string.IsNullOrEmpty(instrument.Symbol))
                .OrderBy(instrument =>
                {
                    if (orderingBySymbol.TryGetValue(instrument.Symbol!, out var ordering)) return ordering;
                    return int.MaxValue;
                }).ToList();

            // Then apply ranking as a secondary sort only if there are curated members
            if (_rankingItems != null)
            {
                // Group instruments by their curated ordering to maintain primary sort
                var groupedByCuratedOrder = geInstruments
                    .GroupBy(instrument =>
                    {
                        if (orderingBySymbol.TryGetValue(instrument.Symbol!, out var ordering)) return ordering;
                        return int.MaxValue;
                    })
                    .OrderBy(g => g.Key)
                    .ToList();

                // Apply ranking within each curated order group
                var result = new List<GeInstrument>();
                foreach (var group in groupedByCuratedOrder)
                {
                    var groupInstruments = group.ToList();
                    if (group.Key == int.MaxValue)
                    {
                        // For instruments without curated ordering, sort them purely by ranking
                        groupInstruments = RankingHelper.Rank(_rankingItems, groupInstruments);
                    }
                    else
                    {
                        // For instruments with same curated ordering, apply ranking as secondary sort
                        var rankingMap = _rankingItems
                            .GroupBy(r => new { r.Symbol, r.Venue })
                            .ToDictionary(
                                g => g.Key,
                                g => g.Max(r => r.Amount)
                            );

                        groupInstruments = groupInstruments
                            .Select(instrument => new
                            {
                                Instrument = instrument,
                                Rank = rankingMap.TryGetValue(
                                    new { instrument.Symbol, instrument.Venue },
                                    out var amount)
                                    ? amount
                                    : 0m
                            })
                            .OrderByDescending(x => x.Rank)
                            .Select(x => x.Instrument)
                            .ToList();
                    }

                    result.AddRange(groupInstruments);
                }

                geInstruments = result;
            }
        }

        return geInstruments;
    }
}