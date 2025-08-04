using Pi.SetMarketData.Application.Interfaces.MarketProfileFundamentals;
using Pi.SetMarketData.Application.Queries.MarketProfileFundamentals;
using Pi.SetMarketData.Application.Queries.MarketProfileOverview;
using Pi.SetMarketData.Application.Services.MarketDataController.MarketProfileOverviewService;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketProfileOverview
{
    public class MarketProfileOverviewRequestHandler
        : GetMarketProfileOverviewRequestAbstractHandler
    {
        private readonly IMongoService<Domain.Entities.CorporateAction> _corporateActionService;
        private readonly ITimescaleService<RealtimeMarketData> _timescaleService;
        private readonly ICacheService _cacheService;
        private readonly IEntityCacheService _entityCacheService;

        public MarketProfileOverviewRequestHandler(
            IMongoService<Domain.Entities.CorporateAction> corporateActionService,
            ITimescaleService<RealtimeMarketData> timescaleService,
            ICacheService cacheService,
            IEntityCacheService entityCacheService
        )
        {
            _corporateActionService = corporateActionService;
            _timescaleService = timescaleService;
            _cacheService = cacheService;
            _entityCacheService = entityCacheService;
        }

        protected override async Task<PostMarketProfileOverviewResponse> Handle(
            PostMarketProfileOverViewRequest request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var _venue = request.data.Venue ?? "";
                var _symbol = request.data.Symbol ?? "";
                var _currentDateTime = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);

                var _instrument = await _entityCacheService.GetInstrumentBySymbol(_symbol) ?? new Domain.Entities.Instrument();

                var _corporateAction =
                    await _corporateActionService.GetByFilterAsync(target =>
                        target.OrderBookId == _instrument.OrderBookId
                    ) ?? new Domain.Entities.CorporateAction();

                var tradingSignOrderBookId =
                    _venue == "Equity"
                        ? _instrument.OrderBookId
                        : _instrument.UnderlyingOrderBookID;

                var _tradingSign = new Domain.Entities.TradingSign();
                if (tradingSignOrderBookId.HasValue)
                    _tradingSign = await _entityCacheService.GetTradingSignByOrderBookId(tradingSignOrderBookId.Value)
                        ?? new Domain.Entities.TradingSign();

                var _setVenueMapping = await _entityCacheService.GetSetVenueMapping(_venue) ?? new SetVenueMapping();

                // var high52W =
                //     await _timescaleService.GetSelectedHighestValueAsync(
                //         target =>
                //             target.Symbol == _symbol
                //             && target.Venue == _venue
                //             && _currentDateTime >= target.DateTime
                //             && _currentDateTime.AddYears(-1) < target.DateTime,
                //         target => target.Price,
                //         target => target.Price
                //     ) ?? 0;

                // var low52W =
                //     await _timescaleService.GetSelectedLowestValueAsync(
                // target =>
                //     target.Symbol == _symbol
                //     && target.Venue == _venue
                //     && _currentDateTime >= target.DateTime
                //     && _currentDateTime.AddYears(-1) < target.DateTime,
                //         target => target.Price,
                //         target => target.Price
                //     ) ?? 0;

                var (high52W, low52W) = await _timescaleService.GetHighestLowest52Weeks(_symbol, _venue, _currentDateTime.DateTime);

                var marketStreamingValue = await _cacheService.GetAsync<MarketStreamingResponse>(
                    $"{CacheKey.StreamingBody}{_instrument.OrderBookId}"
                )?? new MarketStreamingResponse();

                var result = new MarketProfileOverviewService()
                    .WithInstruments(_instrument)
                    .WithMarketProfileParams(
                        new MarketProfileOverviewParams { High52W = high52W, Low52W = low52W }
                    )
                    .WithVenueMapping(_setVenueMapping)
                    .WithCorporateAction(_corporateAction)
                    .WithTradingSign(_tradingSign)
                    .WithMarketStreamingResponse(marketStreamingValue)
                    .WithIsTfex(string.Equals(_instrument.Venue, "Derivative", StringComparison.OrdinalIgnoreCase))
                    .GetResult();

                return new PostMarketProfileOverviewResponse(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
