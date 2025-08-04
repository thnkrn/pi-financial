using System.Text.Json.Serialization;

namespace Pi.GlobalMarketDataRealTime.Domain.Models.Request;

// ReSharper disable once UnusedType.Global
public class MarketInstrumentSearchRequest
{
    [JsonPropertyName("instrumentType")] public string? InstrumentType { get; set; }

    [JsonPropertyName("keyword")] public string? Keyword { get; set; }
}