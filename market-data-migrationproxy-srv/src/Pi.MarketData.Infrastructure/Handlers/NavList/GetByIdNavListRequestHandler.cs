using Pi.MarketData.Application.Interfaces.NavList;
using Pi.MarketData.Application.Queries.NavList;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.NavList;

public class GetByIdNavListRequestHandler : GetByIdNavListRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.NavList> _navListService;

    public GetByIdNavListRequestHandler(IMongoService<Domain.Entities.NavList> navListService)
    {
        _navListService = navListService;
    }

    protected override async Task<GetByIdNavListResponse> Handle(GetByIdNavListRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _navListService.GetByIdAsync(request.id);
            return new GetByIdNavListResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}