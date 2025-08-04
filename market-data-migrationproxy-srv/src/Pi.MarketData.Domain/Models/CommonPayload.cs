using Newtonsoft.Json;

namespace Pi.MarketData.Domain.Models;

public class CommonPayload
{
    [JsonProperty("symbol")] public string? Symbol { get; set; }

    [JsonProperty("venue")] public string? Venue { get; set; }
}