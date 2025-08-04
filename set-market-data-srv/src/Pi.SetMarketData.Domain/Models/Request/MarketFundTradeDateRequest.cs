using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Domain.Models.Request;

public class MarketFundTradeDateRequest
{
    [JsonPropertyName("side")]
    public string? Side { get; set; }

    [JsonPropertyName("switchSymbol")]
    public string? SwitchSymbol { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }
}