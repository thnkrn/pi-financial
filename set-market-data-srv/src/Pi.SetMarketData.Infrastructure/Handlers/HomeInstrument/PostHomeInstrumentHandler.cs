using Pi.SetMarketData.Application.Interfaces.MarketHomeInstrument;
using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Application.Services.MarketData.HomeInstrument;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.SetMarketData.Infrastructure.Handlers.HomeInstrument
{
    public class PostHomeInstrumentHandler : PostHomeInstrumentAbstractHandler
    {
        private readonly IMongoService<CuratedList> _curatedListService;
        private readonly ICacheService _cacheService;
        private readonly IEntityCacheService _entityCacheService;
        private readonly ITimescaleService<RealtimeMarketData> _realtimeMarketDataService;

        public PostHomeInstrumentHandler(
            IMongoService<CuratedList> curatedListService,
            ICacheService cacheService,
            IEntityCacheService entityCacheService,
            ITimescaleService<RealtimeMarketData> realtimeMarketDataService
        )
        {
            _curatedListService = curatedListService;
            _cacheService = cacheService;
            _entityCacheService = entityCacheService;
            _realtimeMarketDataService = realtimeMarketDataService;
        }

        protected override async Task<PostHomeInstrumentsResponse> Handle(
            PostHomeInstrumentsRequest request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var curatedList = await _curatedListService.GetByFilterAsync(target =>
                    target.Name == request.Data.ListName &&
                    target.RelevantTo == request.Data.RelevantTo
                ) ?? new CuratedList
                {
                    Name = request.Data.ListName,
                    RelevantTo = request.Data.RelevantTo
                };

                var instruments = (await _entityCacheService.GetInstrument(curatedList)).ToList();

                var logos = instruments.Select(instrument =>
                {
                    var logoMarket = instrument.Venue;
                    if (string.IsNullOrEmpty(logoMarket))
                        logoMarket = "SET";
                    return LogoHelper.GetLogoUrl(logoMarket, instrument.SecurityType ?? string.Empty, instrument.Symbol ?? string.Empty);
                }).ToList();

                var cacheKeys = instruments
                    .Where(instrument => instrument?.OrderBookId != null)
                    .Select(instrument => $"{CacheKey.SetStreamingBody}{instrument.OrderBookId}")
                    .ToList();

                var priceResponsesResult = await _cacheService.GetBatchAsync<PriceResponse>(cacheKeys);

                var priceResponses = cacheKeys.Select(key =>
                    priceResponsesResult.TryGetValue(key, out var priceResponse)
                        ? priceResponse
                        : null
                ).ToList();

                var result = HomeInstrumentService.GetResult(
                    instruments.ToList(),
                    priceResponses,
                    logos
                );

                return new PostHomeInstrumentsResponse(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

