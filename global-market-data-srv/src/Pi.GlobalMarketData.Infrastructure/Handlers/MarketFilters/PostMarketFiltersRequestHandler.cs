using Pi.GlobalMarketData.Application.Interfaces.MarketFilters;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketFilters;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketFilters;

public class PostMarketFiltersRequestHandler : PostMarketFiltersRequestAbstractHandler
{
    private readonly IMongoService<CuratedFilter> _curatedFilterService;

    public PostMarketFiltersRequestHandler(IMongoService<CuratedFilter> curatedFilterService)
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
            var curatedFilterList = curatedFilter?.ToList() ?? [];

            foreach (var filter in curatedFilterList)
            {
                if (
                    filter.GroupName == request.Data.GroupName
                    && filter.SubGroupName == request.Data.SubGroupName
                )
                {
                    curatedFilters.Add(filter);
                }
            }

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
