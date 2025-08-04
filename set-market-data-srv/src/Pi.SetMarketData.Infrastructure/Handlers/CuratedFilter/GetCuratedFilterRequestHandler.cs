using Pi.SetMarketData.Application.Interfaces;
using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Application.Services.MarketDataManagement;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers;

public class GetCuratedFilterRequestHandler : GetCuratedFilterRequestAbstractHandler
{
    private readonly IMongoService<CuratedFilter> _curatedFilterService;
    private readonly IMongoService<CuratedList> _curatedListService;

    public GetCuratedFilterRequestHandler
    (
        IMongoService<CuratedFilter> curatedFilterService,
        IMongoService<CuratedList> curatedListService
    )
    {
        _curatedFilterService = curatedFilterService;
        _curatedListService = curatedListService;
    }

    protected override async Task<GetCuratedFilterResponse> Handle(GetCuratedFilterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var curatedFilter = await _curatedFilterService.GetAllByFilterAsync(target =>
                (request.GroupName == null || target.GroupName == request.GroupName) &&
                (request.SubGroupName == null || target.SubGroupName == request.SubGroupName)
            );

            var curatedListId = curatedFilter.Select(target => target.CuratedListId).ToHashSet();

            var curatedList = await _curatedListService.GetAllByFilterAsync(target =>
                curatedListId.Contains(target.CuratedListId)
            );

            var result = CuratedFilterService.GetResult(curatedFilter, curatedList);
            return new GetCuratedFilterResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}