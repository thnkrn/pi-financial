using Pi.MarketData.Application.Commands.TradingSign;
using Pi.MarketData.Application.Interfaces.TradingSign;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.TradingSign;

public class DeleteTradingSignRequestHandler : DeleteTradingSignRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.TradingSign> _tradingSignService;

    public DeleteTradingSignRequestHandler(IMongoService<Domain.Entities.TradingSign> tradingSignService)
    {
        _tradingSignService = tradingSignService;
    }

    protected override async Task<DeleteTradingSignResponse> Handle(DeleteTradingSignRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _tradingSignService.DeleteAsync(request.id);
            return new DeleteTradingSignResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}