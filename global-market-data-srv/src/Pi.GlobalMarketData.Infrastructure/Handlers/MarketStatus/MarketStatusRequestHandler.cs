using Pi.GlobalMarketData.Application.Interfaces.MarketStatus;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketStatus;
using Pi.GlobalMarketData.Domain.Models.Response;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketStatus;

public class MarketStatusRequestHandler : PostMarketStatusRequestAbstactHandler
{
    private readonly IEntityCacheService _entityCacheService;
    private readonly IVenueMappingHelper _venueMappingHelper;

    public MarketStatusRequestHandler
    (
        IEntityCacheService entityCacheService,
        IVenueMappingHelper venueMappingHelper
    )
    {
        _entityCacheService = entityCacheService;
        _venueMappingHelper = venueMappingHelper;
    }

    protected override async Task<PostMarketStatusResponse> Handle(
        PostMarketStatusRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var response = new MarketStatusResponse();
            var _startTimeUtc = DateTime.UtcNow;

            string market = await _venueMappingHelper.GetExchangeNameFromVenueMapping(request.data.Market);
            var whiteList = await _entityCacheService.GetWhiteListByMarket(market);

            if (whiteList != null)
            {
                string exchange =  whiteList.Exchange ?? string.Empty;
                var _marketScheduleData = await _entityCacheService.GetMarketSessionStatus(exchange, _startTimeUtc);

                if (_marketScheduleData != null)
                    response = MarketStatusService.GetResult(_marketScheduleData);
            }

            return new PostMarketStatusResponse(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
