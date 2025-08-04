using Pi.SetMarketData.Application.Commands.NavList;
using Pi.SetMarketData.Application.Interfaces.NavList;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.NavList;

public class UpdateNavListRequestHandler : UpdateNavListRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.NavList> _NavListService;

    public UpdateNavListRequestHandler(IMongoService<Domain.Entities.NavList> NavListService)
    {
        _NavListService = NavListService;
    }

    protected override async Task<UpdateNavListResponse> Handle(UpdateNavListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _NavListService.UpdateAsync(request.id, request.NavList);
            return new UpdateNavListResponse(true, request.NavList);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}