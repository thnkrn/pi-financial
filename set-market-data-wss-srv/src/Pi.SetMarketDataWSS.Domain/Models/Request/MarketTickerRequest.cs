using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Request;

public class MarketTickerParameter
{
    [JsonPropertyName("symbol")] public string? Symbol { get; set; }

    [JsonPropertyName("venue")] public string? Venue { get; set; }
}

public class MarketTickerRequest
{
    [JsonPropertyName("param")] public List<MarketTickerParameter>? Param { get; set; }
}