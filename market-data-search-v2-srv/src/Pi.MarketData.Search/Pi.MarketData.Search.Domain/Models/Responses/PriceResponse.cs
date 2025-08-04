using Newtonsoft.Json;

namespace Pi.MarketData.Search.Domain.Models.Responses;

public class PriceResponse
{
    [JsonProperty("price")]
    public string? Price { get; set; }

    [JsonProperty("priceChanged")]
    public string? PriceChanged { get; set; }

    [JsonProperty("priceChangedRate")]
    public string? PriceChangedRate { get; set; }

    [JsonProperty("totalVolume")]
    public string? TotalVolume { get; set; }

    [JsonProperty("totalAmount")]
    public string? TotalAmount { get; set; }
}