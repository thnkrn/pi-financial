using Pi.GlobalMarketData.Application.Interfaces.MarketHomeInstrument;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.HomeInstrument;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using Microsoft.Extensions.Logging;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.HomeInstrument
{
    public class PostHomeInstrumentHandler : PostHomeInstrumentAbstractHandler
    {
        private readonly IMongoService<CuratedList> _curatedListService;
        private readonly IEntityCacheService _entityCacheService;
        private readonly ICacheService _cacheService;

        private readonly ILogger<PostHomeInstrumentHandler> _logger;

        public PostHomeInstrumentHandler(
            IMongoService<CuratedList> curatedListService,
            IEntityCacheService entityCacheService,
            ICacheService cacheService,
            ILogger<PostHomeInstrumentHandler> logger
        )
        {
            _curatedListService = curatedListService;
            _entityCacheService = entityCacheService;
            _cacheService = cacheService;
            _logger = logger;
        }

        protected override async Task<PostHomeInstrumentResponse> Handle(
            PostHomeInstrumentRequest request,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("PostHomeInstrumentHandler: {ListName} {RelevantTo}", request.Data.ListName, request.Data.RelevantTo);
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

                var instruments = await _entityCacheService.GetGeInstruments(curatedList);

                _logger.LogInformation("PostHomeInstrumentHandler > instruments: {Count}", instruments.Count());

                var logos = instruments.Select(instrument =>
                {
                    var logoMarket = instrument.Venue;
                    if (string.IsNullOrEmpty(logoMarket))
                        logoMarket = "SET";
                    return LogoHelper.GetLogoUrl(logoMarket, instrument.Symbol ?? "");
                }).ToList();

                var cacheKeys = instruments
                    .Where(instrument => instrument.Symbol != null)
                    .Select(instrument => $"{CacheKey.GeStreamingBody}{instrument.Symbol}")
                    .ToList();

                var priceResponseResult = await _cacheService.GetBatchAsync<PriceResponse>(cacheKeys);

                var priceResponses = HomeInstrumentService.GetResult(
                    instruments.ToList(),
                    priceResponseResult,
                    logos
                );

                _logger.LogInformation(
                    "PostHomeInstrumentHandler {ListName} {RelevantTo} > marketStreamingResult: {StramingCount} >  result.Response?.InstrumentList: {Count}",
                    request.Data.ListName,
                    request.Data.RelevantTo,
                    priceResponseResult.Count(),
                    priceResponses.Response?.InstrumentList?.Count()
                );

                return new PostHomeInstrumentResponse(priceResponses);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
