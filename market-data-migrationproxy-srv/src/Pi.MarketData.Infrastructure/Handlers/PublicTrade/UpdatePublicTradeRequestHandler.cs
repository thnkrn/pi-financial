using Pi.MarketData.Application.Commands.PublicTrade;
using Pi.MarketData.Application.Interfaces.PublicTrade;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.PublicTrade;

public class UpdatePublicTradeRequestHandler : UpdatePublicTradeRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.PublicTrade> _PublicTradeService;

    public UpdatePublicTradeRequestHandler(IMongoService<Domain.Entities.PublicTrade> PublicTradeService)
    {
        _PublicTradeService = PublicTradeService;
    }

    protected override async Task<UpdatePublicTradeResponse> Handle(UpdatePublicTradeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _PublicTradeService.UpdateAsync(request.id, request.PublicTrade);
            return new UpdatePublicTradeResponse(true, request.PublicTrade);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}