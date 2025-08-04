using Pi.MarketData.Application.Interfaces.TradingSign;
using Pi.MarketData.Application.Queries.TradingSign;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.TradingSign;

public class GetTradingSignRequestHandler : GetTradingSignRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.TradingSign> _TradingSignService;

    public GetTradingSignRequestHandler(IMongoService<Domain.Entities.TradingSign> TradingSignService)
    {
        _TradingSignService = TradingSignService;
    }

    protected override async Task<GetTradingSignResponse> Handle(GetTradingSignRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _TradingSignService.GetAllAsync();
            return new GetTradingSignResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}