using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Request;

public class MarketFiltersRequest
{
    [JsonPropertyName("groupName")] public string? GroupName { get; set; }

    [JsonPropertyName("subGroupName")] public string? SubGroupName { get; set; }
}