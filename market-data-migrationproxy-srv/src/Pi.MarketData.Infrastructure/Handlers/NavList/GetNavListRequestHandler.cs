using Pi.MarketData.Application.Interfaces.NavList;
using Pi.MarketData.Application.Queries.NavList;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.NavList;

public class GetNavListRequestHandler : GetNavListRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.NavList> _NavListService;

    public GetNavListRequestHandler(IMongoService<Domain.Entities.NavList> NavListService)
    {
        _NavListService = NavListService;
    }

    protected override async Task<GetNavListResponse> Handle(GetNavListRequest request,
        CancellationToken cancellationToken)
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