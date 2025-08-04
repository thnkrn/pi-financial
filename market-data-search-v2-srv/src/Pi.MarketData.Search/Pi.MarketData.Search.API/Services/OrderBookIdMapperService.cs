using Pi.MarketData.Search.API.Interfaces;
using Pi.MarketData.Search.Domain.ConstantConfigurations;
using Pi.MarketData.Search.Domain.Entities;
using Pi.MarketData.Search.Domain.Models;
using Pi.MarketData.Search.Infrastructure.Interfaces.Mongo;
using Pi.MarketData.Search.Infrastructure.Interfaces.Redis;

namespace Pi.MarketData.Search.API.Services;

public class OrderBookIdMapperService : IOrderBookIdMapperService
{
    private readonly IMongoService<Instrument> _instrumentService;
    private readonly ICacheService _cacheService;
    public OrderBookIdMapperService
    (
        IMongoService<Instrument> instrumentService,
        ICacheService cacheService
    )
    {
        _instrumentService = instrumentService;
        _cacheService = cacheService;
    }

    public async Task<Tuple<List<string>, List<string>, Dictionary<string, string>>> MapCacheKeysFromSymbols(UserInstrumentFavoriteResponse response)
    {
        var setSymbols = response.Response.InstrumentCategoryList   // TODO: need to change from Symbol to OrderBookId
            .SelectMany(category => category.InstrumentList)
            .Where(instrument =>
                DomainVenue.SetVenue.Contains(instrument.Venue)
                || DomainVenue.TfexVenue.Contains(instrument.Venue)
            )
            .Select(instrument => instrument.Symbol)
            .ToList();
        var instruments = await GetInstrumentsAsync(setSymbols);

        var setKeys = instruments.Select(instrument => $"{CacheKey.SetStreamingBody}{instrument.OrderBookId}").ToList();
        var symbolMap = instruments
            .Where(i => !string.IsNullOrEmpty(i.Symbol))
            .ToDictionary(i => i.Symbol!, i => $"{CacheKey.SetStreamingBody}{i.OrderBookId}")
            ?? [];

        var geSymbols = response.Response.InstrumentCategoryList
            .SelectMany(category => category.InstrumentList)
            .Where(instrument => DomainVenue.GeVenue.Contains(instrument.Venue))
            .Select(instrument => instrument.Symbol)
            .ToList();
        var geKeys = geSymbols.Select(symbol => $"{CacheKey.GeStreamingBody}{symbol}").ToList();

        return Tuple.Create(setKeys, geKeys, symbolMap);
    }

    private async Task<List<Instrument>> GetInstrumentsAsync(List<string> symbols)
    {
        var cacheKey = symbols.Select(symbol => $"{CacheKey.OrderBookId}{symbol}").ToList();
        var instrumentsDict = await _cacheService.GetBatchAsync<Instrument>(cacheKey);

        var missingSymbol = new HashSet<string>();
        foreach (var kvp in instrumentsDict)
        {
            if (kvp.Value == null)
            {
                var symbol = kvp.Key.Replace(CacheKey.OrderBookId, string.Empty);
                missingSymbol.Add(symbol);
            }
        }

        var instruments = await _instrumentService.GetAllByFilterAsync(target =>  // Query for missing Instrument
            missingSymbol.Contains(target.Symbol ?? "")
        );
        await CacheOrderBookId(instruments);

        var result = instruments.ToList();
        var nonNullCachedInstruments = instrumentsDict.Values
            .Where(v => v != null)
            .Cast<Instrument>();

        result.AddRange(nonNullCachedInstruments);
        return result;
    }

    private async Task CacheOrderBookId(IEnumerable<Instrument> instruments)
    {
        foreach (var instrument in instruments)
        {
            await _cacheService.SetAsync($"{CacheKey.OrderBookId}{instrument.Symbol}", instrument, new TimeSpan(0, 4, 0, 0));
        }
    }
}