using Pi.SetMarketData.Application.Interfaces.PublicTrade;
using Pi.SetMarketData.Application.Queries.PublicTrade;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.PublicTrade;

public class GetPublicTradeRequestHandler: GetPublicTradeRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.PublicTrade> _PublicTradeService;
    
    public GetPublicTradeRequestHandler(IMongoService<Domain.Entities.PublicTrade> PublicTradeService)
    {
        _PublicTradeService = PublicTradeService;
    }
    
    protected override async Task<GetPublicTradeResponse> Handle(GetPublicTradeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _PublicTradeService.GetAllAsync();
            return new GetPublicTradeResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}