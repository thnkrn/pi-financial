using Pi.SetMarketData.Application.Queries.TradingSign;
using Pi.SetMarketData.Application.Interfaces.TradingSign;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.TradingSign;

public class GetByIdTradingSignRequestHandler : GetByIdTradingSignRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.TradingSign> _tradingSignService;

    public GetByIdTradingSignRequestHandler(IMongoService<Domain.Entities.TradingSign> tradingSignService)
    {
        _tradingSignService = tradingSignService;
    }

    protected override async Task<GetByIdTradingSignResponse> Handle(GetByIdTradingSignRequest request, CancellationToken cancellationToken)
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