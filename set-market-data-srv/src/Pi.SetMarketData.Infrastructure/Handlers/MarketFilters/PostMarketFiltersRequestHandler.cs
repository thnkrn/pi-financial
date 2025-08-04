using Pi.SetMarketData.Application.Interfaces.MarketFilters;
using Pi.SetMarketData.Application.Queries.MarketFilters;
using Pi.SetMarketData.Application.Services.MarketData.MarketFilters;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketFilters;

public class PostMarketFiltersRequestHandler : PostMarketFiltersRequestAbstractHandler
{
    private readonly IMongoService<CuratedFilter> _curatedFilterService;

    public PostMarketFiltersRequestHandler(
        IMongoService<CuratedFilter> curatedFilterService
    )
    {
        _curatedFilterService = curatedFilterService;
    }

    protected override async Task<PostMarketFiltersResponse> Handle(
        PostMarketFiltersRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            List<CuratedFilter> curatedFilters = [];
            var curatedFilter = await _curatedFilterService.GetAllAsync();
            var curatedFilterList = curatedFilter.ToList();

            foreach (var filter in curatedFilterList)
                if (filter.GroupName == request.Data.GroupName && filter.SubGroupName == request.Data.SubGroupName)
                    curatedFilters.Add(filter);

            var result = MarketFiltersService.GetResult(curatedFilters);

            return new PostMarketFiltersResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}