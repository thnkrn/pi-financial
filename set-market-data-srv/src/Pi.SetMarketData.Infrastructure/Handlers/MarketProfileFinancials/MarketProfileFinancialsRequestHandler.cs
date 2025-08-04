using Pi.SetMarketData.Application.Interfaces.MarketProfileFinancials;
using Pi.SetMarketData.Application.Queries.MarketProfileFinancials;
using Pi.SetMarketData.Application.Services.MarketData.MarketProfileFinancials;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketProfileFinancials;

public class MarketProfileFinancialsRequestHandler
    : GetMarketProfileFinancialsRequestAbstractHandler
{
    private readonly IEntityCacheService _entityCacheService;
    private readonly int _limited;

    /// <summary>
    /// </summary>
    /// <param name="incomeStatementService"></param>
    /// <param name="morningStarEtfsService"></param>
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
            string symbol = request.Data.Symbol ?? string.Empty;
            string venue = request.Data.Venue ?? string.Empty;

            var venueMapping = await _entityCacheService.GetSetVenueMapping(venue) ?? new Domain.Entities.SetVenueMapping();
            var exchangeId = venueMapping.ExchangeIdMs;
            var morningStarStocks = await _entityCacheService.GetMorningStarStocks(symbol, exchangeId ?? "");

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
