using Pi.MarketData.Application.Interfaces.TradingSign;
using Pi.MarketData.Application.Queries.TradingSign;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.TradingSign;

public class GetByIdTradingSignRequestHandler : GetByIdTradingSignRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.TradingSign> _tradingSignService;

    public GetByIdTradingSignRequestHandler(IMongoService<Domain.Entities.TradingSign> tradingSignService)
    {
        _tradingSignService = tradingSignService;
    }

    protected override async Task<GetByIdTradingSignResponse> Handle(GetByIdTradingSignRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tradingSignService.GetByIdAsync(request.id);
            return new GetByIdTradingSignResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}