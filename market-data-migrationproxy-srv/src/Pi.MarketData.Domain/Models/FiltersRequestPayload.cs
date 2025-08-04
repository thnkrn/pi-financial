using Newtonsoft.Json;

namespace Pi.MarketData.Domain.Models;

public class FiltersRequestPayload
{
    [JsonProperty("filterList")] public List<int>? FilterList { get; set; }

    [JsonProperty("groupName")] public string? GroupName { get; set; }

    [JsonProperty("subGroupName")] public string? SubGroupName { get; set; }
}