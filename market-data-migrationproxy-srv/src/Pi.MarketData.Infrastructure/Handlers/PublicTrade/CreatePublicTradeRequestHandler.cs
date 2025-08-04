using Pi.MarketData.Application.Commands.PublicTrade;
using Pi.MarketData.Application.Interfaces.PublicTrade;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.PublicTrade;

public class CreatePublicTradeRequestHandler : CreatePublicTradeRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.PublicTrade> _publicTradeService;

    public CreatePublicTradeRequestHandler(IMongoService<Domain.Entities.PublicTrade> publicTradeService)
    {
        _publicTradeService = publicTradeService;
    }

    protected override async Task<CreatePublicTradeResponse> Handle(CreatePublicTradeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _publicTradeService.CreateAsync(request.PublicTrade);
            return new CreatePublicTradeResponse(true, request.PublicTrade);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}