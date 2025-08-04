using Pi.MarketData.Application.Interfaces.MarketProfileFundamentals;
using Pi.MarketData.Application.Queries.MarketProfileFundamentals;
using Pi.MarketData.Application.Queries.MarketProfileOverview;
using Pi.MarketData.Application.Services.MarketData.MarketProfileOverview;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.MarketProfileOverview;

public class MarketProfileOverviewRequestHandler
    : GetMarketProfileOverviewRequestAbstractHandler
{
    // private readonly IMongoService<MarketProfileOverviewResponse> _marketProfileOverviewResponse;
    private readonly IMongoService<Domain.Entities.CorporateAction> _corporateActionService;
    private readonly IMongoService<Domain.Entities.TradingSign> _tradingSignService;


    public MarketProfileOverviewRequestHandler(
        IMongoService<Domain.Entities.CorporateAction> corporateActionService,
        IMongoService<Domain.Entities.TradingSign> tradingSignService
        // IMongoService<MarketProfileOverviewResponse> marketProfileOverviewResponse
    )
    {
        _corporateActionService = corporateActionService;
        _tradingSignService = tradingSignService;
        // _marketProfileOverviewResponse = marketProfileOverviewResponse;
    }

    protected override async Task<PostMarketProfileOverviewResponse> Handle(
        PostMarketProfileOverViewRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var corporateActionsResponse = await _corporateActionService.GetAllAsync();
            var tradingSignResponse = await _tradingSignService.GetAllAsync();
            var convertedResponse = corporateActionsResponse.ToList();
            var convertedTradingSignResponse = tradingSignResponse.ToList();

            // TODO: Mapping the rest values for API response
            // var response = await _marketProfileOverviewResponse.GetMongoBySymbolAsync(
            //     request.data.Symbol ?? ""
            // );

            var result = MarketProfileOverviewService.GetResult(convertedResponse, convertedTradingSignResponse);
            return new PostMarketProfileOverviewResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}