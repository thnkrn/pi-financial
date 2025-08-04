using Pi.SetMarketData.Application.Commands.PublicTrade;
using Pi.SetMarketData.Application.Interfaces.PublicTrade;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.PublicTrade;

public class DeletePublicTradeRequestHandler : DeletePublicTradeRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.PublicTrade> _publicTradeService;

    public DeletePublicTradeRequestHandler(IMongoService<Domain.Entities.PublicTrade> publicTradeService)
    {
        _publicTradeService = publicTradeService;
    }

    protected override async Task<DeletePublicTradeResponse> Handle(DeletePublicTradeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _publicTradeService.DeleteAsync(request.id);
            return new DeletePublicTradeResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}