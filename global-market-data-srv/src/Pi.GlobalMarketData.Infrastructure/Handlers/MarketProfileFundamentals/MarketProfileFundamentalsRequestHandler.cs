using Pi.GlobalMarketData.Application.Interfaces.MarketProfileFundamentals;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketProfileFundamentals;
using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Models.Response;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketProfileFundamentals;

public class MarketProfileFundamentalsRequestHandler
    : GetMarketProfileFundamentalsRequestAbstractHandler
{
    private readonly ICacheService _cacheService;
    private readonly IEntityCacheService _entityCacheService;

    /// <summary>
    /// </summary>
    /// <param name="cacheService"></param>
    /// <param name="entityCacheService"></param>
    public MarketProfileFundamentalsRequestHandler(
        ICacheService cacheService,
        IEntityCacheService entityCacheService
    )
    {
        _cacheService = cacheService;
        _entityCacheService = entityCacheService;
    }

    protected override async Task<PostMarketProfileFundamentalsResponse> Handle(
        PostMarketProfileFundamentalsRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var symbol = DataManipulation.ToMorningStarSymbol(
                request.Data.Symbol,
                request.Data.Venue
            );

            var venueMapping = await _entityCacheService.GetGeVenueMappingByVenue(request.Data.Venue ?? "") ?? new Domain.Entities.GeVenueMapping();

            var venue = venueMapping.ExchangeIdMs;
            var morningStarStocks = _entityCacheService.GetMorningStarStocks(symbol, venue);
            var morningStarEtfs = _entityCacheService.GetMorningStarEtfs(symbol, venue);

            var streamingBody = _cacheService.GetAsync<StreamingBody>(
                $"{CacheKey.StreamingBody}{request.Data.Symbol}"
            );

            Task.WaitAll(
                [morningStarEtfs, morningStarEtfs, streamingBody],
                cancellationToken: cancellationToken
            );

            var result = MarketProfileFundamentalsService.GetResult(
                await morningStarStocks,
                await morningStarEtfs,
                await streamingBody
            );

            return new PostMarketProfileFundamentalsResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}