using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Helpers;

public class MongoDbServices
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="whiteListService"></param>
    /// <param name="marketScheduleService"></param>
    /// <param name="marketSessionStatusService"></param>
    public MongoDbServices(
        IMongoService<WhiteList> whiteListService,
        IMongoService<MarketSchedule> marketScheduleService,
        IMongoService<MarketSessionStatus> marketSessionStatusService)
    {
        WhiteListService = whiteListService;
        MarketScheduleService = marketScheduleService;
        MarketSessionStatusService = marketSessionStatusService;
    }

    public IMongoService<WhiteList> WhiteListService { get; }

    public IMongoService<MarketSchedule> MarketScheduleService { get; }

    public IMongoService<MarketSessionStatus> MarketSessionStatusService { get; }
}