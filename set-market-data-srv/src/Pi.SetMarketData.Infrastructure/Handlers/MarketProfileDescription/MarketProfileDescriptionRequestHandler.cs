using Pi.SetMarketData.Application.Interfaces.MarketProfileDescription;
using Pi.SetMarketData.Application.Queries.MarketProfileDescription;
using Pi.SetMarketData.Application.Services.MarketData.MarketProfileDescription;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketProfileDescription;

public class MarketProfileDescriptionRequestHandler
    : GetMarketProfileDescriptionRequestAbstractHandler
{
    private readonly IEntityCacheService _entityCacheService;
    private readonly HashSet<string> _venueList = new(StringComparer.OrdinalIgnoreCase) { "ARCX", "ARCA", "BATS" };

    /// <summary>
    /// </summary>
    /// <param name="morningStarStocksService"></param>
    /// <param name="setVenueMappingService"></param>
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
            var symbol = request.Data.Symbol ?? string.Empty;
            var venue = request.Data.Venue ?? string.Empty;
            var venueMapping = await _entityCacheService.GetSetVenueMapping(venue) ?? new SetVenueMapping();

            var exchangeId = venueMapping.ExchangeIdMs;
            var morningStarStocks = await _entityCacheService.GetMorningStarStocks(symbol, exchangeId ?? "");
            
            var instrument = await _entityCacheService.GetInstrumentBySymbol(symbol);

            // Logo for response
            var logoMarket = !string.IsNullOrEmpty(venue)
                ? venue
                : venueMapping.Exchange;

            if (string.IsNullOrEmpty(logoMarket))
                logoMarket = "SET";

            // Standardize case
            logoMarket = logoMarket.ToUpper();
            var logoUrl = _venueList.Contains(logoMarket)
                ? LogoHelper.GetLogoUrl(logoMarket, symbol)
                : string.Empty;
            if (instrument != null)
            {
                logoUrl = LogoHelper.GetLogoUrl(logoMarket, instrument.SecurityType ?? string.Empty, symbol);
            }

            var result = MarketProfileDescriptionService.GetResult(morningStarStocks, logoUrl);

            return new PostMarketProfileDescriptionResponse(result);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.Message, ex);
        }
    }
}
