using Pi.GlobalMarketData.NonRealTimeDataHandler.Helpers;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;

public interface IMorningStarCenterDataService
{
    Task MorningStarService(MorningStarCenterDataHelper helper);
}