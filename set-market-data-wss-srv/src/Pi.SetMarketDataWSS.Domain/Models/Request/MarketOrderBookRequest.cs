using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Request;

public class MarketOrderBookRequest
{
    [JsonPropertyName("symbolVenueList")] public List<SymbolVenueList>? SymbolVenueList { get; set; }
}

public class SymbolVenueList
{
    [JsonPropertyName("symbol")] public string? Symbol { get; set; }

    [JsonPropertyName("venue")] public string? Venue { get; set; }
}