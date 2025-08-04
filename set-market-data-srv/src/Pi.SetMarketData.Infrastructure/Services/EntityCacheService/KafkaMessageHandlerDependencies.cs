using System.Diagnostics.CodeAnalysis;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Services.EntityCacheService;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
public class EntityCacheServiceDependencies
{
    /// <summary>
    /// </summary>
    /// <param name="setVenueMappingService"></param>
    /// <param name="morningStarStocksService"></param>
    /// <param name="instrumentService"></param>
    /// <param name="instrumentDetailService"></param>
    /// <param name="tradingSignService"></param>
    /// <param name="curatedFilterService"></param>
    /// <param name="curatedMemberService"></param>
    /// <param name="curatedListService"></param>
    [SuppressMessage("SonarQube", "S107")]
    public EntityCacheServiceDependencies(IMongoService<SetVenueMapping> setVenueMappingService,
        IMongoService<MorningStarStocks> morningStarStocksService,
        IMongoService<Instrument> instrumentService,
        IMongoService<InstrumentDetail> instrumentDetailService,
        IMongoService<TradingSign> tradingSignService,
        IMongoService<CuratedFilter> curatedFilterService,
        IMongoService<CuratedMember> curatedMemberService,
        IMongoService<CuratedList> curatedListService)
    {
        SetVenueMappingService = setVenueMappingService;
        MorningStarStocksService = morningStarStocksService;
        InstrumentService = instrumentService;
        InstrumentDetailService = instrumentDetailService;
        TradingSignService = tradingSignService;
        CuratedFilterService = curatedFilterService;
        CuratedMemberService = curatedMemberService;
        CuratedListService = curatedListService;
    }

    public IMongoService<SetVenueMapping> SetVenueMappingService { get; set; }
    public IMongoService<MorningStarStocks> MorningStarStocksService { get; set; }
    public IMongoService<Instrument> InstrumentService { get; set; }
    public IMongoService<InstrumentDetail> InstrumentDetailService { get; set; }
    public IMongoService<TradingSign> TradingSignService { get; set; }
    public IMongoService<CuratedFilter> CuratedFilterService { get; set; }
    public IMongoService<CuratedMember> CuratedMemberService { get; set; }
    public IMongoService<CuratedList> CuratedListService { get; set; }
}