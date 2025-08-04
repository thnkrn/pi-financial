using Pi.GlobalMarketData.Application.Interfaces.MarketSchedule;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketSchedules;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketSchedules;

public class MarketSchedulesRequestHandler : PostMarketSchedulesRequestAbstactHandler
{
    private readonly IMongoService<MarketSchedule> _marketScheduleService;
    private readonly IVenueMappingHelper _venueMappingHelper;

    public MarketSchedulesRequestHandler(
        IMongoService<MarketSchedule> marketScheduleService,
        IVenueMappingHelper venueMappingHelper
        )
    {
        _marketScheduleService = marketScheduleService;
        _venueMappingHelper = venueMappingHelper;
    }

    protected override async Task<PostMarketScheduleResponse> Handle(
        PostMarketScheduleRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var symbol = request.data.Symbol;
            var _startTimeUtc = DateTime.UtcNow.Date;
            var _endTimeUtc = DateTime.UtcNow.Date.AddDays(1).AddHours(23).AddMinutes(59).AddSeconds(59);
            var venue = await _venueMappingHelper.GetExchangeNameFromVenueMapping(request.data.Venue);

            var _marketScheduleData = await _marketScheduleService.GetAllByFilterAsync(target =>
                !string.IsNullOrEmpty(target.Symbol) &&
                target.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase) &&
                (target.Exchange ?? "").Equals(venue, StringComparison.OrdinalIgnoreCase) &&
                target.UTCStartTime >= _startTimeUtc &&
                target.UTCStartTime <= _endTimeUtc
            );

            _marketScheduleData = _marketScheduleData.OrderBy(e => e.UTCStartTime).Take(10);

            var result = MarketSchedulesService.GetResult(_marketScheduleData);
            return new PostMarketScheduleResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
