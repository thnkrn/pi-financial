using Pi.SetMarketData.Application.Commands.NavList;
using Pi.SetMarketData.Application.Interfaces.NavList;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.NavList;

public class DeleteNavListRequestHandler : DeleteNavListRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.NavList> _navListService;

    public DeleteNavListRequestHandler(IMongoService<Domain.Entities.NavList> navListService)
    {
        _navListService = navListService;
    }

    protected override async Task<DeleteNavListResponse> Handle(DeleteNavListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _navListService.DeleteAsync(request.id);
            return new DeleteNavListResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}