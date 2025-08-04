using Newtonsoft.Json;

namespace Pi.MarketData.Domain.Models.Requests;

public class MarketStatusRequest
{
    [JsonProperty("market")] public string? Market { get; set; }
}