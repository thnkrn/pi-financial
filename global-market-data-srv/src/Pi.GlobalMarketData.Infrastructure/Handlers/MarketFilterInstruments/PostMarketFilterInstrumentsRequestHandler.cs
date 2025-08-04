using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketData.Application.Interfaces.MarketFilterInstruments;
using Pi.GlobalMarketData.Application.Queries.MarketFilterInstruments;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketFilterInstruments;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketFilterInstruments
{
    public class PostMarketFilterInstrumentsRequestHandler : PostMarketFilterInstrumentsAbstractHandler
    {
        private readonly IMongoService<CuratedFilter> _curatedFilterService;
        private readonly IEntityCacheService _entityCacheService;
        private readonly ICacheService _cacheService;

        public PostMarketFilterInstrumentsRequestHandler(
            IMongoService<CuratedFilter> curatedFilterService,
            IEntityCacheService entityCacheService,
            ICacheService cacheService
        )
        {
            _curatedFilterService = curatedFilterService;
            _entityCacheService = entityCacheService;
            _cacheService = cacheService;
        }

        protected override async Task<PostMarketFilterInstrumentsResponse> Handle(
            PostMarketFilterInstrumentsRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var selectedCuratedMembers = new List<CuratedMember>();
                var curatedFilters = new List<CuratedFilter>();
                var instruments = new List<Domain.Entities.GeInstrument>();

                foreach (var filterId in request.Data.FilterList ?? [])
                {
                    var curatedFilter = await _curatedFilterService.GetByFilterAsync(x => x.FilterId == filterId);
                    if ((curatedFilter == null) && (curatedFilter?.CuratedListId == null)) continue;
                    var geInstrument = await _entityCacheService.GetGeInstruments(curatedFilter);
                    instruments.AddRange(geInstrument);
                }

                var logos = await Task.Run(() =>
                    instruments.AsParallel().AsOrdered().Select(instrument =>
                    {
                        var logoMarket = instrument.Venue;
                        if (string.IsNullOrEmpty(logoMarket))
                            logoMarket = "SET";

                        var logo = LogoHelper.GetLogoUrl(logoMarket, instrument.Symbol ?? "");

                        return logo;
                    }).ToList()
                );

                var cacheKeys = instruments
                    .Where(instrument => instrument.Symbol != null)
                    .Select(instrument => $"{CacheKey.GeStreamingBody}{instrument.Symbol}")
                    .ToList();

                var priceResponseResult = await _cacheService.GetBatchAsync<PriceResponse>(cacheKeys);

                var priceResponses = cacheKeys.Select(key =>
                    priceResponseResult.TryGetValue(key, out var streamingBody)
                        ? streamingBody
                        : null
                ).ToList();

                var result = MarketFilterInstrumentsService.GetResult
                (
                    curatedFilters,
                    instruments,
                    priceResponses,
                    logos
                );

                return new PostMarketFilterInstrumentsResponse(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
