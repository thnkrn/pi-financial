using Pi.SetMarketData.Application.Queries.NavList;
using Pi.SetMarketData.Application.Interfaces.NavList;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.NavList;

public class GetByIdNavListRequestHandler : GetByIdNavListRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.NavList> _navListService;

    public GetByIdNavListRequestHandler(IMongoService<Domain.Entities.NavList> navListService)
    {
        _navListService = navListService;
    }

    protected override async Task<GetByIdNavListResponse> Handle(GetByIdNavListRequest request, CancellationToken cancellationToken)
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