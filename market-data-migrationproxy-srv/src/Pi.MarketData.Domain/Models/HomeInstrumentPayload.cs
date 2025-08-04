using Newtonsoft.Json;

namespace Pi.MarketData.Domain.Models;

public class HomeInstrumentPayload
{
    [JsonProperty("relevantTo")]
    public string? RelevantTo { get; set; }

    [JsonProperty("listName")]
    public string? ListName { get; set; }
}