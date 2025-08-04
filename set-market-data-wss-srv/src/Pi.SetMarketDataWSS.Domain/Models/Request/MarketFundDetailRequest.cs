using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Request;

public class MarketFundDetailRequest
{
    [JsonPropertyName("symbol")] public string? Symbol { get; set; }
}