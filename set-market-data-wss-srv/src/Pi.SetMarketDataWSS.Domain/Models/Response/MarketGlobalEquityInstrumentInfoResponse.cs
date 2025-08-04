using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Response;

public class GlobalEquityInstrumentInfoResponse
{
    [JsonPropertyName("minimalPriceIncrement")]
    public string? MinimalPriceIncrement { get; set; }

    [JsonPropertyName("minimalQuantityIncrement")]
    public string? MinimalQuantityIncrement { get; set; }

    [JsonPropertyName("currency")] public string? Currency { get; set; }
}

public class MarketGlobalEquityInstrumentInfoResponse
{
    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("response")] public GlobalEquityInstrumentInfoResponse? Response { get; set; }

    [JsonPropertyName("debugStack")] public string? DebugStack { get; set; }
}