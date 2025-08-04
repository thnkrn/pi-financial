using Pi.SetMarketData.Application.Interfaces;
using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers;

public class GetCuratedListRequestHandler: GetCuratedListRequestAbstractHandler
{
    private readonly IMongoService<CuratedList> _CuratedListService;
    
    public GetCuratedListRequestHandler(IMongoService<CuratedList> CuratedListService)
    {
        _CuratedListService = CuratedListService;
    }
    
    protected override async Task<GetCuratedListResponse> Handle(GetCuratedListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _CuratedListService.GetAllAsync();
            return new GetCuratedListResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}