using Pi.GlobalMarketData.Application.Interfaces.MarketProfileDescription;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketProfileDescription;
using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketProfileDescription;

public class MarketProfileDescriptionRequestHandler
    : GetMarketProfileDescriptionRequestAbstractHandler
{
    private readonly HashSet<string> _venueList = new(StringComparer.OrdinalIgnoreCase) { "ARCX", "ARCA", "BATS" };
    private readonly IEntityCacheService _entityCacheService;

    /// <summary>
    /// </summary>
    /// <param name="entityCacheService"></param>
    public MarketProfileDescriptionRequestHandler(
        IEntityCacheService entityCacheService
    )
    {
        _entityCacheService = entityCacheService;
    }

    protected override async Task<PostMarketProfileDescriptionResponse> Handle(
        PostMarketProfileDescriptionRequest request,
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

            var morningStarStocks = await _entityCacheService.GetMorningStarStocks(symbol, venue);

            var morningStarEtfs = await _entityCacheService.GetMorningStarEtfs(symbol, venue);

            var logoMarket = request.Data.Venue ?? venue;

            // Standardize case
            logoMarket = logoMarket.ToUpper();
            var logoUrl = _venueList.Contains(logoMarket)
                ? LogoHelper.GetLogoUrl(logoMarket, request.Data.Symbol ?? symbol)
                : string.Empty;

            var result = MarketProfileDescriptionService.GetResult(
                morningStarStocks,
                morningStarEtfs,
                logoUrl
            );

            return new PostMarketProfileDescriptionResponse(result);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.Message, ex);
        }
    }
}