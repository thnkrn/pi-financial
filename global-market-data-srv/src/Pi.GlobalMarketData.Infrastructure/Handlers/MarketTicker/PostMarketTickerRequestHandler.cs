using Pi.GlobalMarketData.Application.Interfaces.MarketTicker;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketTicker;
using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketTicker;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class MarketTickerServices
{
#pragma warning disable CS8618
    public IMongoService<Domain.Entities.GeInstrument> InstrumentService { get; set; }
    public IMongoService<ExchangeTimezone> ExchangeTimezoneService { get; set; }
    public IMongoService<MorningStarStocks> MorningStarStocksService { get; set; }
    public IMongoService<MorningStarEtfs> MorningStarEtfsService { get; set; }
    public IMongoService<GeVenueMapping> VenueMappingService { get; set; }
    public IMongoService<MarketSchedule> MarketScheduleService { get; set; }
    public ITimescaleService<RealtimeMarketData> TimescaleService { get; set; }
    public ICacheService CacheService { get; set; }
#pragma warning restore CS8618
}

public class PostMarketTickerRequestHandler : PostMarketTickerRequestAbstractHandler
{
    private readonly ICacheService _cacheService;
    private readonly IMongoService<ExchangeTimezone> _exchangeTimezoneService;
    private readonly IMongoService<Domain.Entities.GeInstrument> _instrumentService;
    private readonly IMongoService<MarketSchedule> _marketScheduleService;
    private readonly IMongoService<MorningStarEtfs> _morningStarEtfsService;
    private readonly IMongoService<MorningStarStocks> _morningStarStocksService;
    private readonly ITimescaleService<RealtimeMarketData> _timescaleService;
    private readonly IMongoService<GeVenueMapping> _venueMappingService;

    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    public PostMarketTickerRequestHandler(MarketTickerServices services)
    {
        _instrumentService = services.InstrumentService;
        _exchangeTimezoneService = services.ExchangeTimezoneService;
        _morningStarStocksService = services.MorningStarStocksService;
        _morningStarEtfsService = services.MorningStarEtfsService;
        _venueMappingService = services.VenueMappingService;
        _marketScheduleService = services.MarketScheduleService;
        _timescaleService = services.TimescaleService;
        _cacheService = services.CacheService;
    }

    protected override async Task<PostMarketTickerResponse> Handle(PostMarketTickerRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var param = request.Data.Param ?? [];
            var currentDateTime = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);

            List<string> symbolList = [];
            List<string> venueList = [];
            List<string> instrumentCategoryList = [];
            List<double> high52WList = [];
            List<double> low52WList = [];

            List<Domain.Entities.GeInstrument> instrumentList = [];
            List<ExchangeTimezone> exchangeTimezoneList = [];
            List<MorningStarStocks> morningStarStockList = [];
            List<StreamingBody> streamingBodyList = [];
            List<string> logos = [];
            List<MarketSchedule> marketSchedules = [];
            var currentTime = DateTime.UtcNow;

            for (var i = 0; i < request.Data.Param?.Count; i++)
            {
                var symbol = param[i].Symbol;
                var venue = param[i].Venue ?? string.Empty;
                var morningStarSymbol = DataManipulation.ToMorningStarSymbol(symbol, venue);

                // Define task
                var venueMappingTask =
                     _venueMappingService.GetByFilterAsync(target => target.VenueCode == venue);
                var instrumentTask = _instrumentService.GetByFilterAsync(target =>
                        target.Symbol == symbol && target.Venue == venue
                    );
                var exchangeTimezoneTask = _exchangeTimezoneService.GetByFilterAsync(target =>
                        target.Exchange == venue
                    );
                var streamingBodyTask = _cacheService.GetAsync<StreamingBody>($"{CacheKey.StreamingBody}{symbol}");
                var marketScheduleTask = _marketScheduleService.GetByFilterAsync(target =>
                    !string.IsNullOrEmpty(target.Symbol)
                    && target.Symbol.Equals(symbol)
                    && target.UTCStartTime.HasValue
                    && target.UTCEndTime.HasValue
                    && currentTime >= target.UTCStartTime.Value
                    && currentTime <= target.UTCEndTime.Value
                );

                // Wait for all task done
                await Task.WhenAll(venueMappingTask, instrumentTask, exchangeTimezoneTask, streamingBodyTask, marketScheduleTask);

                //Get task result
                var venueMapping = venueMappingTask.Result ?? new GeVenueMapping();
                var instrument = instrumentTask.Result ?? new Domain.Entities.GeInstrument();
                var exchangeTimezone = exchangeTimezoneTask.Result ?? new ExchangeTimezone();
                var exchangeId = venueMapping.ExchangeIdMs;
                var streamingBody = streamingBodyTask.Result ?? new StreamingBody();
                var marketSchedule = marketScheduleTask.Result ?? new MarketSchedule();
               
                var morningStarStocksTask = _morningStarStocksService.GetByFilterAsync(target =>
                    string.Equals(target.Symbol, morningStarSymbol, StringComparison.OrdinalIgnoreCase)
                    && target.ExchangeId.Equals(exchangeId, StringComparison.OrdinalIgnoreCase)
                );
                var morningStarEtfsTask = _morningStarEtfsService.GetByFilterAsync(target =>
                    string.Equals(target.Symbol, morningStarSymbol, StringComparison.OrdinalIgnoreCase)
                    && target.ExchangeId.Equals(exchangeId, StringComparison.OrdinalIgnoreCase)
                );
                var timescaleTask = _timescaleService.GetHighestLowest52Weeks(symbol, venue, currentDateTime.DateTime);

                // Wait for all 2nd batch task done
                Task.WaitAll(morningStarStocksTask, morningStarEtfsTask, timescaleTask);

                var morningStarStocks = morningStarStocksTask.Result;
                var morningStarEtfs = morningStarEtfsTask.Result;
                var (high52W, low52W) = timescaleTask.Result;

                var instrumentCategory = string.Empty;

                if (morningStarEtfs != null)
                    instrumentCategory = StockDetail.ETFs.Value;
                else if (morningStarStocks != null) instrumentCategory = StockDetail.Stock.Value;

                

                // Logo for response
                var logo = LogoHelper.GetLogoUrl(venue, symbol ?? string.Empty);

                symbolList.Add(symbol ?? string.Empty);
                venueList.Add(venue);
                instrumentCategoryList.Add(instrumentCategory);
                high52WList.Add(high52W);
                low52WList.Add(low52W);
                instrumentList.Add(instrument);
                exchangeTimezoneList.Add(exchangeTimezone);
                morningStarStockList.Add(morningStarStocks ?? new MorningStarStocks());
                streamingBodyList.Add(streamingBody);
                logos.Add(logo);
                marketSchedules.Add(marketSchedule);
            }

            var result = new MarketTickerService()
                .SetTickerPriceParams(high52WList, low52WList)
                .SetTickerParams(
                    symbolList,
                    venueList,
                    instrumentList,
                    exchangeTimezoneList,
                    morningStarStockList,
                    streamingBodyList
                )
                .SetLogoParams(logos)
                .SetInstrumentCategory(instrumentCategoryList)
                .SetMarketSchedule(marketSchedules)
                .GetResult();

            return new PostMarketTickerResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}