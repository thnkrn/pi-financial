using System.Text.Json.Serialization;

namespace Pi.GlobalMarketData.Domain.Models.Request;

public class MarketDerivativeInformationRequest
{
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }
}