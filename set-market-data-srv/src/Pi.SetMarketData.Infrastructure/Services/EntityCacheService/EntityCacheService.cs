using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Pi.SetMarketData.Application.Helper;
using Pi.SetMarketData.Application.Services.MarketData.HomeInstrument;
using Pi.SetMarketData.Application.Services.MarketData.MarketFilterInstruments;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Converters;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketData.Infrastructure.Services.EntityCacheService;

public class EntityCacheService : IEntityCacheService
{
    private readonly ICacheService _cacheService;
    private readonly TimeSpan _cacheType1;
    private readonly TimeSpan _cacheType2;
    private readonly TimeSpan _cacheType3;
    private readonly TimeSpan _cacheType4;
    private readonly IMongoService<CuratedFilter> _curatedFilterService;
    private readonly IMongoService<CuratedList> _curatedListService;
    private readonly IMongoService<CuratedMember> _curatedMemberService;
    private readonly IMongoService<InstrumentDetail> _instrumentDetailService;
    private readonly IMongoService<Instrument> _instrumentService;

    private readonly JsonSerializerSettings _jsonSettings = new() { Converters = [new ObjectIdConverter()] };
    private readonly IMongoService<MorningStarStocks> _morningStarStocksService;
    private readonly IMongoService<SetVenueMapping> _setVenueMappingService;
    private readonly IMongoService<TradingSign> _tradingSignService;
    private List<RankingItem> _rankingItems;

    public EntityCacheService(ICacheService cacheService,
        EntityCacheServiceDependencies dependencies)
    {
        _cacheService = cacheService;
        _setVenueMappingService = dependencies.SetVenueMappingService;
        _morningStarStocksService = dependencies.MorningStarStocksService;
        _instrumentService = dependencies.InstrumentService;
        _instrumentDetailService = dependencies.InstrumentDetailService;
        _tradingSignService = dependencies.TradingSignService;
        _curatedFilterService = dependencies.CuratedFilterService;
        _curatedMemberService = dependencies.CuratedMemberService;
        _curatedListService = dependencies.CuratedListService;
        _rankingItems = _cacheService.GetAsync<List<RankingItem>>($"{CacheKey.RankingItem}:SET").GetAwaiter().GetResult() ?? [];

        var configuration = ConfigurationHelper.GetConfiguration();
        var type1 = configuration.GetValue<int?>(ConfigurationKeys.EntityCacheOption1) ?? 1;
        var type2 = configuration.GetValue<int?>(ConfigurationKeys.EntityCacheOption2) ?? 1;
        var type3 = configuration.GetValue<int?>(ConfigurationKeys.EntityCacheOption3) ?? 1;
        var type4 = configuration.GetValue<int?>(ConfigurationKeys.EntityCacheOption4) ?? 1;

        _cacheType1 = new TimeSpan(0, 0, type1, 0);
        _cacheType2 = new TimeSpan(0, type2, 0, 0);
        _cacheType3 = new TimeSpan(0, type3, 0, 0);
        _cacheType4 = new TimeSpan(type4, 0, 0, 0);
    }

    public async Task UpdateRankingItem(List<RankingItem> rankingItems)
    {
        _rankingItems = rankingItems;
        await _cacheService.SetAsync($"{CacheKey.RankingItem}:SET", _rankingItems);
    }

    public async Task<SetVenueMapping?> GetSetVenueMapping(string venue)
    {
        var response = await _cacheService.GetAsync<SetVenueMapping>($"{CacheKey.SetVenueMapping}{venue}");

        if (response == null)
        {
            var setVenueMapping = await _setVenueMappingService.GetByFilterAsync(target =>
                target.VenueCode == venue
            );
            if (setVenueMapping != null)
                await _cacheService.SetAsync(
                    $"{CacheKey.SetVenueMapping}{venue}",
                    JsonConvert.SerializeObject(setVenueMapping, _jsonSettings),
                    _cacheType4
                );
            return setVenueMapping;
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
                    JsonConvert.SerializeObject(morningStarStocks, _jsonSettings),
                    _cacheType2
                );
            return morningStarStocks;
        }

        return response;
    }

    public async Task<Instrument?> GetInstrumentBySymbol(string? symbol)
    {
        if (string.IsNullOrEmpty(symbol)) return null;
        var response = await _cacheService.GetAsync<Instrument>($"{CacheKey.Instrument}{symbol}");

        if (response == null)
        {
            var instrument = await _instrumentService.GetByFilterAsync(target =>
                target.Symbol == symbol
            );
            if (instrument != null)
                await _cacheService.SetAsync(
                    $"{CacheKey.Instrument}{symbol}",
                    JsonConvert.SerializeObject(instrument, _jsonSettings),
                    _cacheType1
                );
            return instrument;
        }

        return response;
    }

    public async Task<IEnumerable<Instrument>> GetInstrument(CuratedList curatedList)
    {
        var hash = CacheKeyHelper.GenerateCacheKey(curatedList);
        var cacheKey = $"{CacheKey.Instrument}:{hash}";
        var instruments = await _cacheService.GetAsync<IEnumerable<Instrument>>(cacheKey);
        instruments ??= await PreCacheInstrument(curatedList, new TimeSpan(0, 8, 0, 0));
        return instruments;
    }

    public async Task<IEnumerable<Instrument>> GetInstrument(CuratedFilter curatedFilter)
    {
        var hash = CacheKeyHelper.GenerateCacheKey(curatedFilter);
        var cacheKey = $"{CacheKey.Instrument}:{hash}";
        var instruments = await _cacheService.GetAsync<IEnumerable<Instrument>>(cacheKey);
        instruments ??= await PreCacheInstrument(curatedFilter, new TimeSpan(0, 8, 0, 0));
        return instruments;
    }

    public async Task<IEnumerable<Instrument>> PreCacheInstrument(CuratedList curatedList, TimeSpan cacheTime)
    {
        return await PreCacheSortedInstrument(curatedList, cacheTime);
    }

    public async Task<IEnumerable<Instrument>> PreCacheInstrument(CuratedFilter curatedFilter, TimeSpan cacheTime)
    {
        return await PreCacheSortedInstrument(curatedFilter, cacheTime);
    }

    public async Task<IEnumerable<Instrument>> PreCacheSortedInstrument(CuratedList curatedList, TimeSpan cacheTime)
    {
        var (curatedFilter, ignoreCuratedMember) = HomeInstrumentService.GetCuratedFilter(
            curatedList.Name,
            curatedList.RelevantTo
        );

        curatedFilter = await _curatedFilterService.GetByFilterAsync(target =>
            target.FilterName == curatedFilter.FilterName
        ) ?? new CuratedFilter();

        var curatedMembers = await _curatedMemberService.GetAllByFilterAsync(target =>
            target.CuratedListId == curatedList.CuratedListId ||
            target.CuratedListId == curatedFilter.CuratedListId
        ) ?? [];

        var symbols = curatedMembers.Select(member => member.Symbol).ToHashSet();
        var instruments = await _instrumentService.GetAllByFilterAsync(target => symbols.Contains(target.Symbol) && target.Deprecated != true);

        instruments = SortInstrument(instruments.ToList(), curatedMembers.ToList(), ignoreCuratedMember);

        if (instruments.Any())
        {
            var hash = CacheKeyHelper.GenerateCacheKey(curatedList);
            var cacheKey = $"{CacheKey.Instrument}:{hash}";
            await _cacheService.SetAsync(cacheKey, instruments, cacheTime);
        }

        return instruments;
    }

    public async Task<IEnumerable<Instrument>> PreCacheSortedInstrument(CuratedFilter curatedFilter, TimeSpan cacheTime)
    {
        curatedFilter = await _curatedFilterService.GetByFilterAsync(target =>
            target.FilterName == curatedFilter.FilterName
        ) ?? new CuratedFilter();

        var curatedMembers = await _curatedMemberService.GetAllByFilterAsync(target =>
            target.CuratedListId == curatedFilter.CuratedListId
        ) ?? [];

        var instruments = new List<Instrument>();

        // Add Instrument from CuratedMember
        var symbols = curatedMembers.Select(member => member.Symbol).ToHashSet();
        var instrument = await _instrumentService.GetAllByFilterAsync(target => symbols.Contains(target.Symbol) && target.Deprecated != true);
        instruments.AddRange(instrument);

        var filter = MarketFilterInstrumentsService.GetTypeCategory(curatedFilter);
        instrument = await _instrumentService.GetAllByFiltersAsync([filter, target => target.Deprecated != true]);
        instruments.AddRange(instrument);

        instruments = SortInstrument(instruments, curatedMembers.ToList(), false);

        if (instruments.Count != 0)
        {
            var hash = CacheKeyHelper.GenerateCacheKey(curatedFilter);
            var cacheKey = $"{CacheKey.Instrument}:{hash}";
            await _cacheService.SetAsync(cacheKey, instruments, cacheTime);
        }

        return instruments;
    }

    public async Task<InstrumentDetail?> GetInstrumentDetailByOrderBookId(int orderBookId)
    {
        var response = await _cacheService.GetAsync<InstrumentDetail>($"{CacheKey.InstrumentDetail}{orderBookId}");

        if (response == null)
        {
            var instrumentDetail = await _instrumentDetailService.GetByFilterAsync(target =>
                target.OrderBookId == orderBookId
            );
            if (instrumentDetail != null)
                await _cacheService.SetAsync(
                    $"{CacheKey.InstrumentDetail}{orderBookId}",
                    JsonConvert.SerializeObject(instrumentDetail, _jsonSettings),
                    _cacheType1
                );
            return instrumentDetail;
        }

        return response;
    }

    public async Task<Instrument?> GetInstrumentByOrderBookId(int orderBookId)
    {
        var response = await _cacheService.GetAsync<Instrument>($"{CacheKey.Instrument}{orderBookId}");

        if (response == null)
        {
            var instrument = await _instrumentService.GetByFilterAsync(target =>
                target.OrderBookId == orderBookId
            );
            if (instrument != null)
                await _cacheService.SetAsync(
                    $"{CacheKey.Instrument}{orderBookId}",
                    JsonConvert.SerializeObject(instrument, _jsonSettings),
                    _cacheType1
                );
            return instrument;
        }

        return response;
    }

    public async Task<TradingSign?> GetTradingSignByOrderBookId(int orderBookId)
    {
        var response = await _cacheService.GetAsync<TradingSign>($"{CacheKey.TradingSign}{orderBookId}");

        if (response == null)
        {
            var tradingSign = await _tradingSignService.GetByFilterAsync(target =>
                target.OrderBookId == orderBookId
            );
            if (tradingSign != null)
                await _cacheService.SetAsync(
                    $"{CacheKey.TradingSign}{orderBookId}",
                    JsonConvert.SerializeObject(tradingSign, _jsonSettings),
                    _cacheType1
                );
            return tradingSign;
        }

        return response;
    }

    private List<Instrument> SortInstrument
    (
        List<Instrument> instruments,
        List<CuratedMember> curatedMembers,
        bool ignoreCuratedMember
    )
    {
        // remove duplicate instrument
        var uniqueSymbols = new HashSet<string>();
        instruments = instruments
            .Where(i => i.Symbol != null && uniqueSymbols.Add(i.Symbol))
            .ToList();

        // Remove CuratedMember without Ordering
        curatedMembers = curatedMembers.Where(member =>
            member.Ordering.HasValue
            && member.Ordering.Value != 0
        ).ToList();

        // First apply ranking if rankingItems exist and no curated members
        if (_rankingItems.Count != 0 && (curatedMembers.Count == 0 || ignoreCuratedMember))
        {
            instruments = RankingHelper.Rank(_rankingItems, instruments);
            return instruments;
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

            instruments = instruments
                .Where(instrument => !string.IsNullOrEmpty(instrument.Symbol))
                .OrderBy(instrument =>
                {
                    if (orderingBySymbol.TryGetValue(instrument.Symbol!, out var ordering)) return ordering;
                    return int.MaxValue;
                }).ToList();

            // Then apply ranking as a secondary sort only if there are curated members
            if (_rankingItems.Count != 0)
            {
                // Group instruments by their curated ordering to maintain primary sort
                var groupedByCuratedOrder = instruments
                    .GroupBy(instrument =>
                    {
                        if (orderingBySymbol.TryGetValue(instrument.Symbol!, out var ordering)) return ordering;
                        return int.MaxValue;
                    })
                    .OrderBy(g => g.Key)
                    .ToList();

                // Apply ranking within each curated order group
                var result = new List<Instrument>();
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

                instruments = result;
            }
        }

        return instruments;
    }
}
