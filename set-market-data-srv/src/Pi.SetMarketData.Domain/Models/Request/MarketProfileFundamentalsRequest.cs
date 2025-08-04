using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Domain.Models.Request;

public class MarketProfileFundamentalsRequest
{
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("venue")]
    public string? Venue { get; set; }
}