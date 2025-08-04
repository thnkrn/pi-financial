using Pi.SetMarketData.Application.Queries.PublicTrade;
using Pi.SetMarketData.Application.Interfaces.PublicTrade;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.PublicTrade;

public class GetByIdPublicTradeRequestHandler : GetByIdPublicTradeRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.PublicTrade> _publicTradeService;

    public GetByIdPublicTradeRequestHandler(IMongoService<Domain.Entities.PublicTrade> publicTradeService)
    {
        _publicTradeService = publicTradeService;
    }

    protected override async Task<GetByIdPublicTradeResponse> Handle(GetByIdPublicTradeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _publicTradeService.GetByIdAsync(request.id);
            return new GetByIdPublicTradeResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}