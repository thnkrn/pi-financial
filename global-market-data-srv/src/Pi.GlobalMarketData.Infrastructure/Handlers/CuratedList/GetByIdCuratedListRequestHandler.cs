using Pi.GlobalMarketData.Application.Interfaces.CuratedList;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

public class GetByIdCuratedListRequestHandler : GetByIdCuratedListRequestAbstractHandler
{
    private readonly IMongoService<CuratedList> _curatedListService;

    public GetByIdCuratedListRequestHandler(
        IMongoService<CuratedList> curatedListService
    )
    {
        _curatedListService = curatedListService;
    }

    protected override async Task<GetByIdCuratedListResponse> Handle(
        GetByIdCuratedListRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await _curatedListService.GetByIdAsync(request.id);
            return new GetByIdCuratedListResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
