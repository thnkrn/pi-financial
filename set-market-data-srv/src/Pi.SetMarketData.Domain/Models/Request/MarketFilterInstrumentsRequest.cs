using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Domain.Models.Request;

public class MarketFilterInstrumentsRequest
{
    [JsonPropertyName("filterList")]
    public List<int>? FilterList { get; set; }

    [JsonPropertyName("groupName")]
    public string? GroupName { get; set; }

    [JsonPropertyName("subGroupName")]
    public string? SubGroupName { get; set; }
}