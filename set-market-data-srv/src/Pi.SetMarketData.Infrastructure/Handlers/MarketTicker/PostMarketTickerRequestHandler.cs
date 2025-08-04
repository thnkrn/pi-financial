using Pi.SetMarketData.Application.Interfaces.MarketTicker;
using Pi.SetMarketData.Application.Queries.MarketTicker;
using Pi.SetMarketData.Application.Services.MarketData.MarketTicker;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Interfaces.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketTicker;

public class MongoDbServices
{
    /// <summary>
    /// </summary>
    /// <param name="exchangeTimezoneService"></param>
    /// <param name="instrumentDetailService"></param>
    /// <param name="instrumentService"></param>
    /// <param name="morningStarStocksService"></param>
    public MongoDbServices
    (
        IMongoService<ExchangeTimezone> exchangeTimezoneService,
        IMongoService<Domain.Entities.InstrumentDetail> instrumentDetailService,
        IMongoService<Domain.Entities.Instrument> instrumentService,
        IMongoService<MorningStarStocks> morningStarStocksService
    )
    {
        ExchangeTimezoneService = exchangeTimezoneService;
        InstrumentDetailService = instrumentDetailService;
        InstrumentService = instrumentService;
        MorningStarStocksService = morningStarStocksService;
    }

    public IMongoService<ExchangeTimezone> ExchangeTimezoneService { get; }
    public IMongoService<Domain.Entities.InstrumentDetail> InstrumentDetailService { get; }
    public IMongoService<Domain.Entities.Instrument> InstrumentService { get; }
    public IMongoService<MorningStarStocks> MorningStarStocksService { get; }
}

public class PostMarketTickerRequestHandler : PostMarketTickerRequestAbstractHandler
{
    private readonly ICacheService _cacheService;
    private readonly IInstrumentHelper _instrumentHelper;
    private readonly MongoDbServices _mongoDbService;
    private readonly ITimescaleService<RealtimeMarketData> _timescaleService;
    private readonly IEntityCacheService _entityCacheService;

    /// <summary>
    /// </summary>
    /// <param name="timescaleService"></param>
    /// <param name="cacheService"></param>
    /// <param name="instrumentHelper"></param>
    /// <param name="mongoDbServiceParams"></param>
    public PostMarketTickerRequestHandler(
        ITimescaleService<RealtimeMarketData> timescaleService,
        ICacheService cacheService,
        IInstrumentHelper instrumentHelper,
        MongoDbServices mongoDbServiceParams,
        IEntityCacheService entityCacheService
    )
    {
        _timescaleService = timescaleService;
        _cacheService = cacheService;
        _instrumentHelper = instrumentHelper;
        _mongoDbService = mongoDbServiceParams;
        _entityCacheService = entityCacheService;
    }

    protected override async Task<PostMarketTickerResponse> Handle(
        PostMarketTickerRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            // Initialize collections
            var marketStreaming = new List<MarketStreamingResponse>();
            var instruments = new List<Domain.Entities.Instrument>();
            var instrumentDetails = new List<Domain.Entities.InstrumentDetail>();
            var morningStarStockList = new List<MorningStarStocks>();
            var exchangeTimezoneList = new List<ExchangeTimezone>();
            var priceResponseList = new List<PriceResponse>();
            var high52WList = new List<double>();
            var low52WList = new List<double>();
            var logos = new List<string>();
            var isEquity = new List<bool>();

            // Pre-initialize lists with default values
            var count = request.Data.Param?.Count ?? 0;
            for (int i = 0; i < count; i++)
            {
                marketStreaming.Add(new MarketStreamingResponse());
                instruments.Add(new Domain.Entities.Instrument());
                instrumentDetails.Add(new Domain.Entities.InstrumentDetail());
                morningStarStockList.Add(new MorningStarStocks());
                exchangeTimezoneList.Add(new ExchangeTimezone());
                priceResponseList.Add(new PriceResponse());
                high52WList.Add(0);
                low52WList.Add(0);
                logos.Add(string.Empty);
                isEquity.Add(false);
            }

            // Get current datetime once
            var currentDateTime = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);

            // Create tasks for each symbol and execute them in parallel
            var tasks = new Task[count];

            for (var i = 0; i < count; i++)
            {
                int index = i; // Capture the loop variable for use in lambda
                var symbol = request.Data.Param?[index].Symbol ?? string.Empty;
                var venue = request.Data.Param?[index].Venue ?? string.Empty;
                isEquity[index] = (venue == "Equity");

                tasks[index] = Task.Run(async () =>
                {
                    // Get venue mapping
                    var venueMapping = await _entityCacheService.GetSetVenueMapping(venue)
                        ?? new SetVenueMapping();
                    var exchangeId = venueMapping.ExchangeIdMs;

                    // Create all data fetching tasks to run in parallel
                    var instrumentTask = _entityCacheService.GetInstrumentBySymbol(symbol);

                    // Wait for the instrument to get OrderBookId
                    var instrument = await instrumentTask ?? new Domain.Entities.Instrument();
                    instruments[index] = instrument;

                    // Create tasks that depend on instrument
                    var instrumentDetailTask = _cacheService.GetAsync<Domain.Entities.InstrumentDetail>(
                        $"{CacheKey.InstrumentDetail}{instrument.OrderBookId}");
                    var marketStreamingTask = _cacheService.GetAsync<MarketStreamingResponse>(
                        $"{CacheKey.StreamingBody}{instrument.OrderBookId}");

                    // Fetch 52-week high/low
                    var highLowTask = _timescaleService.GetHighestLowest52Weeks(
                        symbol, venue, currentDateTime.DateTime);

                    // Fetch MorningStar stock data
                    var morningStarStockTask = _mongoDbService.MorningStarStocksService.GetByFilterAsync(
                        target => target.Symbol == symbol && target.ExchangeId == exchangeId);

                    // Fetch exchange timezone
                    var exchangeTimezoneTask = _mongoDbService.ExchangeTimezoneService.GetByFilterAsync(
                        target => target.Exchange == venue);

                    // Wait for all tasks to complete
                    await Task.WhenAll(
                        instrumentDetailTask,
                        highLowTask,
                        morningStarStockTask,
                        exchangeTimezoneTask,
                        marketStreamingTask
                    );

                    // Process results
                    var instrumentDetail = await instrumentDetailTask ?? new Domain.Entities.InstrumentDetail();
                    instrumentDetails[index] = instrumentDetail;

                    var (high52W, low52W) = await highLowTask;
                    high52WList[index] = high52W;
                    low52WList[index] = low52W;

                    morningStarStockList[index] = await morningStarStockTask ?? new MorningStarStocks();
                    exchangeTimezoneList[index] = await exchangeTimezoneTask ?? new ExchangeTimezone();

                    // Get underlying price if needed
                    PriceResponse underlyingPriceResponse = new PriceResponse();
                    if (instrumentDetail.UnderlyingOrderBookID.HasValue && instrumentDetail.UnderlyingOrderBookID > 0)
                    {
                        underlyingPriceResponse = await _cacheService.GetAsync<PriceResponse>(
                            $"{CacheKey.SetStreamingBody}{instrumentDetail.UnderlyingOrderBookID}")
                            ?? new PriceResponse();
                    }
                    priceResponseList[index] = underlyingPriceResponse;

                    // Populate market streaming
                    marketStreaming[index] = await marketStreamingTask ?? new MarketStreamingResponse();

                    // Handle friendly name if needed
                    if (string.IsNullOrEmpty(instrument.FriendlyName))
                    {
                        instrument.FriendlyName = await _instrumentHelper.GetFriendlyName(
                            symbol, instrument.InstrumentCategory ?? string.Empty);
                    }

                    // Get symbol for logo
                    var logoSymbol = symbol;
                    var securityType = instrument.SecurityType;
                    if (instrument.UnderlyingOrderBookID != 0)
                    {
                        var underlyingInstrument = await _mongoDbService.InstrumentService.GetByFilterAsync(
                            t => t.OrderBookId == instrument.UnderlyingOrderBookID)
                            ?? new Domain.Entities.Instrument();

                        if (!string.IsNullOrEmpty(underlyingInstrument.Symbol))
                        {
                            logoSymbol = underlyingInstrument.Symbol;
                            securityType = underlyingInstrument.SecurityType;
                        }
                    }

                    // Set logo URL
                    var logoMarket = string.IsNullOrEmpty(venue) ? "SET" : venue;
                    logos[index] = LogoHelper.GetLogoUrl(logoMarket, securityType ?? string.Empty, logoSymbol ?? string.Empty);

                });
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            var result = new MarketTickerService()
                .SetUnderlyingPriceResponse(priceResponseList)
                .SetVenue((request.Data.Param ?? []).Select(x => x.Venue ?? "").ToList())
                .GetResult(
                    marketStreaming,
                    new MarketTickerServiceParams
                    {
                        High52WList = high52WList,
                        Low52WList = low52WList
                    },
                    instruments,
                    instrumentDetails,
                    morningStarStockList,
                    exchangeTimezoneList,
                    logos
                );

            return new PostMarketTickerResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
