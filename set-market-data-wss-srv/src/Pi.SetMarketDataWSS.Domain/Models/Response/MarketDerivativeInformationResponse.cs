using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Response;

public class DerivativeInformationResponse
{
    [JsonPropertyName("securityType")] public string? SecurityType { get; set; }

    [JsonPropertyName("tradingUnit")] public string? TradingUnit { get; set; }

    [JsonPropertyName("minBidUnit")] public string? MinBidUnit { get; set; }

    [JsonPropertyName("multiplier")] public string? Multiplier { get; set; }

    [JsonPropertyName("initialMargin")] public string? InitialMargin { get; set; }
}

public class MarketDerivativeInformationResponse
{
    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("response")] public DerivativeInformationResponse? Response { get; set; }

    [JsonPropertyName("debugStack")] public string? DebugStack { get; set; }
}