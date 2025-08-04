using Pi.MarketData.Domain.Models.Response;
using Pi.SetMarketData.Application.Queries;

namespace Pi.MarketData.Application.Services.MarketDataManagement;

public static class CuratedFilterResponse
{
    public static GetCuratedFilterCombineResponse GetResult(IEnumerable<GetCuratedFilterResponse?> responses)
    {
        var curatedFilterList = responses
            .Where(response => response?.Data != null)
            .SelectMany(response => response?.Data ?? [])
            .ToList();

        var groupedData = curatedFilterList.GroupBy(curatedFilter => new { curatedFilter.GroupName, curatedFilter.SubGroupName })
            .Select(group => new CuratedFilterGroup
            {
                GroupName = group.Key.GroupName ?? "",
                SubGroupName = group.Key.SubGroupName ?? "",
                CuratedFilterList = group.ToList()
            });

        return new GetCuratedFilterCombineResponse
        {
            Data = groupedData.ToList()
        };
    }
}