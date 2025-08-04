using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Application.Interfaces;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Infrastructure.Handlers;

public class GetByIdCuratedListRequestHandler : GetByIdCuratedListRequestAbstractHandler
{
    private readonly IMongoService<CuratedList> _curatedListService;

    public GetByIdCuratedListRequestHandler(IMongoService<CuratedList> curatedListService)
    {
        _curatedListService = curatedListService;
    }

    protected override async Task<GetByIdCuratedListResponse> Handle(GetByIdCuratedListRequest request, CancellationToken cancellationToken)
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