using System.Text.Json.Serialization;

namespace Pi.GlobalMarketData.Domain.Models.Request;

public class HomeInstrumentRequest
{
    [JsonPropertyName("listName")]
    public string? ListName { get; set; }

    [JsonPropertyName("relevantTo")]
    public string? RelevantTo { get; set; }
}