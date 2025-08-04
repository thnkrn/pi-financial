using Pi.GlobalMarketData.Application.Interfaces.MarketProfileFinancials;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketProfileFinancials;
using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketProfileFinancials;

public class MarketProfileFinancialsRequestHandler
    : GetMarketProfileFinancialsRequestAbstractHandler
{
    private readonly IEntityCacheService _entityCacheService;
    private readonly int _limited;

    /// <summary>
    /// </summary>
    /// <param name="entityCacheService"></param>
    public MarketProfileFinancialsRequestHandler(
        IEntityCacheService entityCacheService
    )
    {
        _entityCacheService = entityCacheService;
        _limited = 5;
    }

    protected override async Task<PostMarketProfileFinancialsResponse> Handle(
        PostMarketProfileFinancialsRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var symbol = DataManipulation.ToMorningStarSymbol(
                request.Data.Symbol,
                request.Data.Venue
            );

            var venueMapping = await _entityCacheService.GetGeVenueMappingByVenue(request.Data.Venue ?? "") ?? new GeVenueMapping();

            var venue = venueMapping.ExchangeIdMs ?? string.Empty;
            var morningStarStocks = await _entityCacheService.GetMorningStarStocks(symbol, venue);

            var result = MarketProfileFinancialsService.GetResult(morningStarStocks, _limited);

            return new PostMarketProfileFinancialsResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
