using Pi.GlobalMarketData.Application.Interfaces.MarketIndicator;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketIndicator;
using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketIndicator;

public class MarketIndicatorRequestHandler : PostMarketIndicatorRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.GeInstrument> _instrumentService;
    private readonly IMongoService<ExchangeTimezone> _exchangeTimezoneService;
    private readonly ITimescaleService<CandleData> _candlesDataService;
    private readonly ITimescaleService<TechnicalIndicators> _technicalIndicatorsService;

    public MarketIndicatorRequestHandler(
        IMongoService<Domain.Entities.GeInstrument> instrumentService,
        IMongoService<ExchangeTimezone> exchangeTimezoneService,
        ITimescaleService<CandleData> candlesDataService,
        ITimescaleService<TechnicalIndicators> technicalIndicatorsService
    )
    {
        _instrumentService = instrumentService;
        _exchangeTimezoneService = exchangeTimezoneService;
        _candlesDataService = candlesDataService;
        _technicalIndicatorsService = technicalIndicatorsService;
    }

    protected override async Task<PostMarketIndicatorResponse> Handle(
        PostMarketIndicatorRequest request,
        CancellationToken cancellationToken
    )
    {
        var symbol = request.Data.Symbol ?? string.Empty;
        var venue = request.Data.Venue ?? string.Empty;
        var candleType = request.Data.CandleType ?? string.Empty;

        var startDate = DataManipulation.ToUTCDateTime(request.Data.FromTimestamp);
        var endDate = DataManipulation.ToUTCDateTime(request.Data.ToTimestamp);

        var dayCandleLimit = CalculateDayCandleLimit(venue, startDate, endDate);
        //NOTE: If it is 1 day candle, use dateTime.MinValue to query because we already have a limit;
        startDate = dayCandleLimit != null ? DateTime.MinValue : ApplyWeekStartDateSpecialLogic(startDate, endDate);
        
        var limit = dayCandleLimit ?? (request.Data.Limit ?? 5000);
        
        // Define task for parallel call
        var getGEInstrumentTask = _instrumentService.GetByFilterAsync(target =>
            target.Venue == venue && target.Symbol == symbol
        );
        
        var getCandleListTask = _candlesDataService.GetCandlesAsync(
            candleType,
            symbol,
            venue,
            startDate,
            endDate,
            limit
        );
        var getFirstCandleTask = _technicalIndicatorsService.GetFirstCandle(symbol, venue);

        // Wait until all task done
        await Task.WhenAll(getFirstCandleTask, getCandleListTask, getGEInstrumentTask);

        // Get task result
        var candleList = getCandleListTask.Result;
        var firstCandle = getFirstCandleTask.Result;
        var instrument = getGEInstrumentTask.Result ?? new Domain.Entities.GeInstrument();

        var exchangeTimezone =
            await _exchangeTimezoneService.GetByFilterAsync(target =>
                target.Exchange == instrument.Exchange
            ) ?? new ExchangeTimezone();
        
        List<TechnicalIndicators>? indicatorList = null;

        if (!request.Data.CompleteTradingDay)
        {
            indicatorList = await _technicalIndicatorsService.GetIndicatorsAsync(
                candleType,
                symbol,
                venue,
                startDate,
                endDate,
                limit
            );
        }

        try
        {
            var result = MarketIndicatorService.GetResult(
                candleType,
                request.Data,
                instrument,
                exchangeTimezone,
                candleList ?? [],
                indicatorList ?? [],
                firstCandle
            );

            return new PostMarketIndicatorResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private static DateTime ApplyWeekStartDateSpecialLogic(DateTime startDate, DateTime endDate)
    {
        //NOTE: change week duration from 5 days to 7 days
        if (endDate.Date == DateTime.UtcNow.Date && endDate - startDate == TimeSpan.FromDays(5))
            return startDate.AddDays(-2);

        return startDate;
    }
    
    private static int? CalculateDayCandleLimit(string venue, DateTime startDate, DateTime endDate)
    {
        //NOTE: endDate is today with 1 Day Duration => Query for Overview page
        if (endDate.Date == DateTime.UtcNow.Date && endDate - startDate == TimeSpan.FromDays(1))
        {
            //NOTE: HK Market Open 5.5 hours
            if(venue == "HKEX")
                return 165; // (60 min / (2 min candle)) * (5.5 hours)
            
            //NOTE: US Market Open 6.5 hours
            return 195; // (60 min / (2 min candle)) * (6.5 hours)
        }
        
        return null;
    }
}
