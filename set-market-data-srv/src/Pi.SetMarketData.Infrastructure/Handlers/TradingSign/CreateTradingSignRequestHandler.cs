using Pi.SetMarketData.Application.Commands.TradingSign;
using Pi.SetMarketData.Application.Interfaces.TradingSign;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.TradingSign;

public class CreateTradingSignRequestHandler : CreateTradingSignRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.TradingSign> _tradingSignService;

    public CreateTradingSignRequestHandler(IMongoService<Domain.Entities.TradingSign> tradingSignService)
    {
        _tradingSignService = tradingSignService;
    }

    protected override async Task<CreateTradingSignResponse> Handle(CreateTradingSignRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _tradingSignService.CreateAsync(request.TradingSign);
            return new CreateTradingSignResponse(true, request.TradingSign);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}