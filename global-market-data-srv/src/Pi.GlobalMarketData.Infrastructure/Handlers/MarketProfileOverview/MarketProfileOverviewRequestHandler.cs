using Newtonsoft.Json;
using Pi.GlobalMarketData.Application.Interfaces.MarketProfileOverview;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketProfileOverview;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketProfileOverview
{
    public class MarketProfileOverviewRequestHandler
        : PostMarketProfileOverviewRequestAbstractHandler
    {
        private readonly IMongoService<Domain.Entities.GeInstrument> _geInstrumentService;
        private readonly IEntityCacheService _entityCacheService;
        private readonly IMongoService<ExchangeTimezone> _exchangeTimezoneService;
        private readonly ITimescaleService<RealtimeMarketData> _timescaleService;
        private readonly ICacheService _cacheService;
        private readonly IVelexaApiHelper _velexaAPIHelper;

        public MarketProfileOverviewRequestHandler(
            IMongoService<Domain.Entities.GeInstrument> geInstrumentService,
            IEntityCacheService entityCacheService,
            IMongoService<GeVenueMapping> geVenueMappingService,
            IMongoService<ExchangeTimezone> exchangeTimezoneService,
            ITimescaleService<RealtimeMarketData> timescaleService,
            ICacheService cacheService,
            IVelexaApiHelper velexaAPIHelper
        )
        {
            _geInstrumentService = geInstrumentService;
            _entityCacheService = entityCacheService;
            _exchangeTimezoneService = exchangeTimezoneService;
            _timescaleService = timescaleService;
            _cacheService = cacheService;
            _velexaAPIHelper = velexaAPIHelper;
        }

        protected override async Task<PostMarketProfileOverviewResponse> Handle(
            PostMarketProfileOverViewRequest request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var venue = request.data.Venue ?? "";
                var symbol = request.data.Symbol ?? "";
                var currentDateTime = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);

                var instrument =
                    await _geInstrumentService.GetByFilterAsync(target =>
                        target.Venue == request.data.Venue && target.Symbol == request.data.Symbol
                    ) ?? new Domain.Entities.GeInstrument();

                var geVenueMapping = await _entityCacheService.GetGeVenueMappingByVenue(venue) ?? new GeVenueMapping();

                var exchangeTimezone =
                    await _exchangeTimezoneService.GetByFilterAsync(target =>
                        target.Exchange == geVenueMapping.Exchange
                    ) ?? new ExchangeTimezone();

                var (high52W, low52W) = await _timescaleService.GetHighestLowest52Weeks(symbol, venue, currentDateTime.DateTime);
                var streamingBody = await _cacheService.GetAsync<StreamingBody>
                (
                    $"{CacheKey.StreamingBody}{instrument.Symbol}"
                ) ?? new StreamingBody();
                var minOrderSize = await _velexaAPIHelper.getMinimumOrderSize(symbol, venue);
                var result = MarketProfileOverviewService.GetResult(
                    instrument,
                    minOrderSize,
                    geVenueMapping,
                    exchangeTimezone,
                    new MarketProfileOverviewParams { High52W = high52W, Low52W = low52W },
                    streamingBody
                );
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
