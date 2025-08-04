using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Domain.Models.Request;

public class MarketFundDetailRequest
{
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }
}