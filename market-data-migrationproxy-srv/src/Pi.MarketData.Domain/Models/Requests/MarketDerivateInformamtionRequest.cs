using System.Text.Json.Serialization;

namespace Pi.MarketData.Domain.Models.Request;

public class MarketDerivativeInformationRequest
{
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }
}