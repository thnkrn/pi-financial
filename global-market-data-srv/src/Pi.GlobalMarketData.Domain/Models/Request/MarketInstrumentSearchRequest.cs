using System.Text.Json.Serialization;

namespace Pi.GlobalMarketData.Domain.Models.Request;

public class MarketInstrumentSearchRequest
{
    [JsonPropertyName("instrumentType")] public string? InstrumentType { get; set; } // GlobalEquity

    [JsonPropertyName("keyword")] public string? Keyword { get; set; } //  exchange, name, symbol, venue
}