using Pi.SetMarketData.Application.Interfaces.MarketIndicator;
using Pi.SetMarketData.Application.Queries.MarketIndicator;
using Pi.SetMarketData.Application.Services.MarketData.MarketIndicator;
using Pi.SetMarketData.Application.Utils;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketIndicator;

public class MarketIndicatorRequestHandler : PostMarketIndicatorRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;
    private readonly IMongoService<ExchangeTimezone> _exchangeTimezoneService;
    private readonly ITimescaleService<CandleData> _candlesDataService;
    private readonly ITimescaleService<TechnicalIndicators> _technicalIndicatorsService;

    public MarketIndicatorRequestHandler(
        IMongoService<Domain.Entities.Instrument> instrumentService,
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
    
        var tasks = new List<Task>();

        var getInstrumentTask = _instrumentService.GetByFilterAsync(target => target.Symbol == symbol);
        var getFirstCandleTask = _candlesDataService.GetFirstCandle(symbol, venue);
        var getCandleListTask = _candlesDataService.GetCandlesAsync(
            candleType,
            symbol,
            venue,
            limit,
            startDate,
            endDate
        );
        var getExchangeTimezoneTask = _exchangeTimezoneService.GetByFilterAsync(target => target.Exchange == venue);
        var getIndicatorListTask = _technicalIndicatorsService.GetIndicatorsAsync(
            candleType,
            symbol,
            venue,
            startDate,
            endDate,
            limit
        );

        tasks.Add(getInstrumentTask);
        tasks.Add(getFirstCandleTask);
        tasks.Add(getCandleListTask);
        tasks.Add(getExchangeTimezoneTask);
        if (!request.Data.CompleteTradingDay)
            tasks.Add(getIndicatorListTask);
        
        await Task.WhenAll(tasks);

        var instrument = getInstrumentTask.Result?? new Domain.Entities.Instrument(); // TODO: will add venue check after SET venue fixed
        var exchangeTimezone = getExchangeTimezoneTask.Result?? new ExchangeTimezone();
        var firstCandleTime = getFirstCandleTask.Result;
        var candleList = getCandleListTask.Result;

        List<TechnicalIndicators>? indicatorList = null;

        if (!request.Data.CompleteTradingDay)
            indicatorList = getIndicatorListTask.Result;
        
        try
        {
            var result = MarketIndicatorService.GetResult(
                candleType,
                request.Data,
                instrument,
                exchangeTimezone,
                candleList ?? [],
                indicatorList ?? [],
                firstCandleTime
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
            return 150; // (60 min / (2 min candle)) * (5 hours)
        
        return null;
    }
}
