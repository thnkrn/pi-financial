using System.Text.Json.Serialization;

namespace Pi.GlobalMarketData.Domain.Models.Request;

public class MarketProfileFundamentalsRequest
{
    [JsonPropertyName("symbol")] public string? Symbol { get; set; }

    [JsonPropertyName("venue")] public string? Venue { get; set; }
}