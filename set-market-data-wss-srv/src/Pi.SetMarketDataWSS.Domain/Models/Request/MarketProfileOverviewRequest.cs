using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Request;

public class MarketProfileOverviewRequest
{
    [JsonPropertyName("symbol")] public string? Symbol { get; set; }

    [JsonPropertyName("venue")] public string? Venue { get; set; }
}