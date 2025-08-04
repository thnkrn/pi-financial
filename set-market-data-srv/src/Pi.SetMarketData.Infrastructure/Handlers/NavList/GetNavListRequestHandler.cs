using Pi.SetMarketData.Application.Interfaces.NavList;
using Pi.SetMarketData.Application.Queries.NavList;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.NavList;

public class GetNavListRequestHandler: GetNavListRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.NavList> _NavListService;
    
    public GetNavListRequestHandler(IMongoService<Domain.Entities.NavList> NavListService)
    {
        _NavListService = NavListService;
    }
    
    protected override async Task<GetNavListResponse> Handle(GetNavListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _NavListService.GetAllAsync();
            return new GetNavListResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}