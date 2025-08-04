using System.Text.Json.Serialization;

namespace Pi.GlobalMarketDataWSS.Domain.Models.Response;

public class PriceResponse
{
    [JsonPropertyName("price")] public string? Price { get; set; }

    [JsonPropertyName("priceChanged")] public string? PriceChanged { get; set; }

    [JsonPropertyName("priceChangedRate")] public string? PriceChangedRate { get; set; }

    [JsonPropertyName("totalVolume")] public string? TotalVolume { get; set; }

    [JsonPropertyName("totalAmount")] public string? TotalAmount { get; set; }
}