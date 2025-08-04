using System.Text.Json.Serialization;
using Pi.BackofficeService.Application.Models.CuratedManager;

namespace Pi.BackofficeService.Infrastructure.Models.CuratedManager;

public record CuratedFiltersThirdPartyApiResponse
{
    [JsonConstructor]
    public CuratedFiltersThirdPartyApiResponse(List<CuratedFilterData> data)
    {
        Data = data ?? new List<CuratedFilterData>();
    }

    [JsonPropertyName("data")]
    public List<CuratedFilterData> Data { get; }
}

public record CuratedFilterData
{
    [JsonConstructor]
    public CuratedFilterData(string groupName, string subGroupName, List<CuratedFilterItem> curatedFilterList)
    {
        GroupName = groupName ?? string.Empty;
        SubGroupName = subGroupName ?? string.Empty;
        CuratedFilterList = curatedFilterList ?? new List<CuratedFilterItem>();
    }

    [JsonPropertyName("groupName")]
    public string GroupName { get; }

    [JsonPropertyName("subGroupName")]
    public string SubGroupName { get; }

    [JsonPropertyName("curatedFilterList")]
    public List<CuratedFilterItem> CuratedFilterList { get; }
}