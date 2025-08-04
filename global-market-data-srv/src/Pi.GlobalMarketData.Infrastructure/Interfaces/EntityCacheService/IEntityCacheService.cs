using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;

public interface IEntityCacheService
{
    Task UpdateRankingItem(List<RankingItem> rankingItems);
    Task UpdateWhiteList();
    Task<GeVenueMapping?> GetGeVenueMappingByVenue(string venue);
    Task<GeVenueMapping?> GetGeVenueMappingByExchangeIdMs(string exchangeIdMs);
    Task<IEnumerable<GeInstrument>> GetGeInstruments(CuratedList curatedList);
    Task<IEnumerable<GeInstrument>> GetGeInstruments(CuratedFilter curatedFilter);
    Task<IEnumerable<GeInstrument>> PreCacheGeInstrument(CuratedList curatedList, TimeSpan cacheTime);
    Task<IEnumerable<GeInstrument>> PreCacheGeInstrument(CuratedFilter curatedFilter, TimeSpan cacheTime);
    Task<IEnumerable<GeInstrument>> PreCacheSortedGeInstrument(CuratedList curatedList, TimeSpan cacheTime);
    Task<IEnumerable<GeInstrument>> PreCacheSortedGeInstrument(CuratedFilter curatedFilter, TimeSpan cacheTime);
    Task<MorningStarStocks?> GetMorningStarStocks(string symbol, string exchangeId);
    Task<MorningStarEtfs?> GetMorningStarEtfs(string symbol, string exchangeId);
    Task<WhiteList?> GetWhiteList(string symbol, string exchange);
    Task<WhiteList?> GetWhiteListByMarket(string market);
    Task<MarketSchedule> GetMarketSchedule(string? symbol, string exchange, string marketSession, DateTime currentTime);
    Task<MarketSessionStatus?> GetMarketSessionStatus(string exchange, DateTime startTime);
}