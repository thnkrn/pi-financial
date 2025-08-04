using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response.MarketDataManagement;

namespace Pi.SetMarketData.Application.Services.MarketDataManagement;

public static class CuratedFilterService
{
    public static IEnumerable<CuratedFilterResponse> GetResult
    (
        IEnumerable<CuratedFilter> curatedFilters,
        IEnumerable<CuratedList> curatedLists
    )
    {
        return curatedFilters
            .Join(
                curatedLists,
                filter => filter.CuratedListId,
                list => list.CuratedListId,
                (filter, list) => new CuratedFilterResponse
                {
                    Id = filter.Id.ToString(), 
                    FilterId = filter.FilterId, 
                    FilterName = filter.FilterName, 
                    FilterCategory = filter.FilterCategory,
                    FilterType = filter.FilterType,
                    CategoryPriority = filter.CategoryPriority,
                    GroupName = filter.GroupName,
                    SubGroupName = filter.SubGroupName,
                    CuratedListId = filter.CuratedListId,
                    ListName = list.Name,
                    ListSource = list.CuratedType,
                    IsDefault = filter.IsDefault,
                    Highlight = filter.Highlight,
                    Ordering = filter.Ordering
                });
    }
}