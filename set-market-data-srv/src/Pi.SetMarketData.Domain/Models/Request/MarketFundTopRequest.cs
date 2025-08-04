using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Domain.Models.Request;

public class MarketFundTopRequest
{
    [JsonPropertyName("filterList")]
    public List<object>? FilterList { get; set; }

    [JsonPropertyName("interval")]
    public string? Interval { get; set; }

    [JsonPropertyName("subtab")]
    public string? SubTab { get; set; }
}