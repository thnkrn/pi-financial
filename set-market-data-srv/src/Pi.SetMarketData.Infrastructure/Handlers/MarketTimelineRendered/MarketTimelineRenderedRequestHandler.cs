using Pi.SetMarketData.Application.Interfaces.MarketTimelineRendered;
using Pi.SetMarketData.Application.Queries.MarketTimelineRendered;
using Pi.SetMarketData.Application.Services.MarketData.MarketTimelineRendered;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketTimelineRendered;

public class MarketTimelineRenderedRequestHandler : PostMarketTimelineRenderedRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;
    private readonly IMongoService<Domain.Entities.Intermission> _intermissionService;
    private readonly IMongoService<ExchangeTimezone> _exchangeTimezoneService;
    private readonly ITimescaleService<CandleData> _candleDataService;

    public MarketTimelineRenderedRequestHandler
    (
        IMongoService<Domain.Entities.Instrument> instrumentService,
        IMongoService<Domain.Entities.Intermission> intermissionService,
        IMongoService<ExchangeTimezone> exchangeTimezoneService,
        ITimescaleService<CandleData> candleDataService
    )
    {
        _instrumentService = instrumentService;
        _intermissionService = intermissionService;
        _candleDataService = candleDataService;
        _exchangeTimezoneService = exchangeTimezoneService;
    }

    protected override async Task<PostMarketTimelineRenderedResponse> Handle(
        PostMarketTimelineRenderedRequest request,
        CancellationToken cancellationToken
    )
    {
        var instrument = await _instrumentService.GetByFilterAsync(target =>
            target.Symbol == request.Data.Symbol
        ) ?? new Domain.Entities.Instrument
        {
            Symbol = request.Data.Symbol
        };

        var exchangeTimezone = await _exchangeTimezoneService.GetByFilterAsync(target =>
            target.Exchange == request.Data.Venue
        ) ?? new ExchangeTimezone
        {
            Exchange = instrument.Exchange ?? ""
        };

        var intermissions = await GetIntermissionsAsync(instrument.Market ?? "");

        var candlesData = await _candleDataService.GetCandlesAsync
        (
            CandleType.candle1Min,
            request.Data.Symbol ?? "",
            request.Data.Venue ?? "",
            Int32.MaxValue,
            DateTime.Today.Date.ToUniversalTime(),
            DateTime.Today.Date.AddHours(23).AddMinutes(59).AddSeconds(59).ToUniversalTime()
        );

        try
        {
            var result = MarketTimelineRenderedService.GetResult(instrument, intermissions, candlesData, exchangeTimezone);

            return new PostMarketTimelineRenderedResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Domain.Entities.Intermission>> GetIntermissionsAsync(string market)
    {
        var intermissionsType = InstrumentMarketMapper.GetIntermissionType(market);
        var intermissions = new List<Domain.Entities.Intermission>();

        if (intermissionsType == "Derivative Night")
        {
            await AddIntermissionsForDerivativeNight(intermissions);
        }
        else
        {
            await AddIntermissionForType(intermissions, intermissionsType);
        }

        return intermissions;
    }

    private async Task AddIntermissionsForDerivativeNight(List<Domain.Entities.Intermission> intermissions)
    {
        await AddIntermissionForType(intermissions, "Derivative Day");
        await AddIntermissionForType(intermissions, "Derivative Night");
    }

    private async Task AddIntermissionForType(List<Domain.Entities.Intermission> intermissions, string type)
    {
        var intermission = await _intermissionService.GetByFilterAsync(target => target.InstrumentType == type)
            ?? new Domain.Entities.Intermission();
        intermissions.Add(intermission);
    }
}
