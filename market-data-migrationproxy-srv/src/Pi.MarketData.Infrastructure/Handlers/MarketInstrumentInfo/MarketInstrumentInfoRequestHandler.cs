using Pi.MarketData.Application.Interfaces.MarketInstrumentInfo;
using Pi.MarketData.Application.Queries.MarketProfileFinancials;
using Pi.MarketData.Application.Services.MarketDataController.MarketInstrumentInfoService;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.MarketInstrumentInfo;

public class MarketInstrumentInfoRequestHandler : GetMarketInstrumentInfoAbstractHandler
{
    // private readonly IMongoService<MarketInstrumentInfoResponse> _marketInstrumentInfoResponse;
    private readonly IMongoService<Domain.Entities.TradingSign> _tradingSignService;

    public MarketInstrumentInfoRequestHandler(
        // IMongoService<MarketInstrumentInfoResponse> marketInstrumentInfoResponse,
        IMongoService<Domain.Entities.TradingSign> tradingSignService
    )
    {
        // _marketInstrumentInfoResponse = marketInstrumentInfoResponse;
        _tradingSignService = tradingSignService;
    }

    protected override async Task<PostMarketInstrumentInfoResponse> Handle(
        PostMarketInstrumentInfoRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var tradingSignResponse = await _tradingSignService.GetAllAsync();
            var convertedTradingSignResponse = tradingSignResponse.ToList();

            // TODO: Mapping the rest values for API response
            // var response = await _marketInstrumentInfoResponse.GetBySymbolAsync(
            //     request.data.Symbol ?? ""
            // );

            var result = MarketInstrumentInfoService.GetResult(convertedTradingSignResponse);
            return new PostMarketInstrumentInfoResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}