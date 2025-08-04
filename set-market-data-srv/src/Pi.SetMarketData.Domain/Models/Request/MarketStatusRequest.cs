using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Domain.Models.Request;

public class MarketStatusRequest
{
    [JsonPropertyName("market")]
    public string? Market { get; set; }
}