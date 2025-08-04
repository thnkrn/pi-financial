using Pi.MarketData.Application.Commands.NavList;
using Pi.MarketData.Application.Interfaces.NavList;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.NavList;

public class CreateNavListRequestHandler : CreateNavListRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.NavList> _navListService;

    public CreateNavListRequestHandler(IMongoService<Domain.Entities.NavList> navListService)
    {
        _navListService = navListService;
    }

    protected override async Task<CreateNavListResponse> Handle(CreateNavListRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _navListService.CreateAsync(request.NavList);
            return new CreateNavListResponse(true, request.NavList);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}