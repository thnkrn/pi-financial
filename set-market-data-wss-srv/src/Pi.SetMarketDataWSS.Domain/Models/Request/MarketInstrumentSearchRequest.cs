using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Request;

public class MarketInstrumentSearchRequest
{
    [JsonPropertyName("instrumentType")] public string? InstrumentType { get; set; }

    [JsonPropertyName("keyword")] public string? Keyword { get; set; }
}