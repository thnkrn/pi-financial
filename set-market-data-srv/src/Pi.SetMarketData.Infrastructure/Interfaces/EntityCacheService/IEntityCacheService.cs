using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;

public interface IEntityCacheService
{
    Task UpdateRankingItem( List<RankingItem> rankingItems);
    Task<SetVenueMapping?> GetSetVenueMapping(string venue);
    Task<MorningStarStocks?> GetMorningStarStocks(string symbol, string exchangeId);
    Task<Instrument?> GetInstrumentBySymbol(string? symbol);
    Task<IEnumerable<Instrument>> GetInstrument(CuratedList curatedList);
    Task<IEnumerable<Instrument>> GetInstrument(CuratedFilter curatedFilter);
    Task<IEnumerable<Instrument>> PreCacheInstrument(CuratedList curatedList, TimeSpan cacheTime);
    Task<IEnumerable<Instrument>> PreCacheInstrument(CuratedFilter curatedFilter, TimeSpan cacheTime);

    Task<IEnumerable<Instrument>> PreCacheSortedInstrument(CuratedList curatedList, TimeSpan cacheTime);

    Task<IEnumerable<Instrument>> PreCacheSortedInstrument(CuratedFilter curatedFilter, TimeSpan cacheTime);

    Task<InstrumentDetail?> GetInstrumentDetailByOrderBookId(int orderBookId);
    Task<Instrument?> GetInstrumentByOrderBookId(int orderBookId);
    Task<TradingSign?> GetTradingSignByOrderBookId(int orderBookId);
}