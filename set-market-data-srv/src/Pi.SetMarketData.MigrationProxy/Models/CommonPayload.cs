using Newtonsoft.Json;

namespace Pi.SetMarketData.MigrationProxy.Models;

public class CommonPayload
{
    [JsonProperty("symbol")]
    public string? Symbol { get; set; }
    [JsonProperty("venue")]
    public string? Venue { get; set; }
}