using Pi.SetMarketData.Application.Commands.TradingSign;
using Pi.SetMarketData.Application.Interfaces.TradingSign;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.TradingSign;

public class UpdateTradingSignRequestHandler : UpdateTradingSignRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.TradingSign> _TradingSignService;

    public UpdateTradingSignRequestHandler(IMongoService<Domain.Entities.TradingSign> TradingSignService)
    {
        _TradingSignService = TradingSignService;
    }

    protected override async Task<UpdateTradingSignResponse> Handle(UpdateTradingSignRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _TradingSignService.UpdateAsync(request.id, request.TradingSign);
            return new UpdateTradingSignResponse(true, request.TradingSign);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}