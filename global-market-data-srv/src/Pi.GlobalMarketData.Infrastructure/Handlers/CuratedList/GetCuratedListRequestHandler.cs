using Pi.GlobalMarketData.Application.Interfaces.CuratedList;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

public class GetCuratedListRequestHandler : GetCuratedListRequestAbstractHandler
{
    private readonly IMongoService<CuratedList> _curatedListService;

    public GetCuratedListRequestHandler(
        IMongoService<CuratedList> curatedListService
    )
    {
        _curatedListService = curatedListService;
    }

    protected override async Task<GetCuratedListResponse> Handle(
        GetCuratedListRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await _curatedListService.GetAllAsync();
            return new GetCuratedListResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
