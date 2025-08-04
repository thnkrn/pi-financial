using Pi.GlobalMarketData.Application.Interfaces.MarketTimelineRendered;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketTimelineRendered;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketTimelineRendered;

public class MarketTimelineRenderedRequestHandler : PostMarketTimelineRenderedRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.GeInstrument> _instrumentService;
    private readonly IMongoService<ExchangeTimezone> _exchangeTimezoneService;
    private readonly IEntityCacheService _entityCacheService;
    private readonly ITimescaleService<CandleData> _candleDataService;
    private readonly IVenueMappingHelper _venueMappingHelper;

    public MarketTimelineRenderedRequestHandler(
        IMongoService<Domain.Entities.GeInstrument> instrumentService,
        IMongoService<ExchangeTimezone> exchangeTimezoneService,
        IEntityCacheService entityCacheService,
        ITimescaleService<CandleData> candleDataService,
        IVenueMappingHelper venueMappingHelper
    )
    {
        _instrumentService = instrumentService;
        _exchangeTimezoneService = exchangeTimezoneService;
        _entityCacheService = entityCacheService;
        _candleDataService = candleDataService;
        _venueMappingHelper = venueMappingHelper;
    }

    protected override async Task<PostMarketTimelineRenderedResponse> Handle(
        PostMarketTimelineRenderedRequest request,
        CancellationToken cancellationToken
    )
    {
        var venue = await _venueMappingHelper.GetExchangeNameFromVenueMapping(request.Data.Venue);
        var instrument = await _instrumentService.GetByFilterAsync(target =>
               (target.Venue ?? string.Empty).ToUpper() == venue.ToUpper()
            && (target.Symbol ?? string.Empty).ToUpper() == (request.Data.Symbol ?? string.Empty).ToUpper()
        );

        if (instrument == null)
            throw new InvalidDataException("Instrument data not found");

        var exchangeTimezone = await _exchangeTimezoneService.GetByFilterAsync(target =>
            target.Exchange == instrument.Exchange
        ) ?? new ExchangeTimezone
        {
            Exchange = instrument.Exchange ?? ""
        };

        var marketSchedule = await _entityCacheService.GetMarketSchedule(instrument.Symbol, exchangeTimezone.Exchange, "MainSession", DateTime.UtcNow);

        // Default startTime/endTime
        var startTime = DateTime.Today.Date.AddDays(-1).ToUniversalTime();
        var endTime = DateTime.Today.Date.ToUniversalTime();

        // startTime/endTime on MarketSchedule
        if (marketSchedule != null)
        {
            if (marketSchedule.UTCStartTime.HasValue)
            {
                startTime = marketSchedule.UTCStartTime.Value;
            }
            if (marketSchedule.UTCEndTime.HasValue)
            {
                endTime = marketSchedule.UTCEndTime.Value;
            }
        }

        var candlesData = await _candleDataService.GetCandlesAsync
        (
            CandleType.candle1Min,
            request.Data.Symbol ?? "", 
            request.Data.Venue ?? "",
            startTime,
            endTime,
            Int32.MaxValue
        );

        try
        {
            var result = MarketTimelineRenderedService.GetResult(instrument, exchangeTimezone, candlesData);

            return new PostMarketTimelineRenderedResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
