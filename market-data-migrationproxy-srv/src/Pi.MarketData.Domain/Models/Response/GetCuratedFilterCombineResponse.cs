using Newtonsoft.Json;
using Pi.MarketData.Domain.Models.Response.MarketDataManagement;

namespace Pi.MarketData.Domain.Models.Response;

public class GetCuratedFilterCombineResponse
{
    [JsonProperty("data")]
    public IEnumerable<CuratedFilterGroup>? Data { get; set; }
}

public class CuratedFilterGroup
{
    [JsonProperty("groupName")]
    public string GroupName { get; set; } = string.Empty;

    [JsonProperty("subGroupName")]
    public string SubGroupName { get; set; } = string.Empty;

    [JsonProperty("curatedFilterList")]
    public IEnumerable<CuratedFilterResponse>? CuratedFilterList { get; set; }
}