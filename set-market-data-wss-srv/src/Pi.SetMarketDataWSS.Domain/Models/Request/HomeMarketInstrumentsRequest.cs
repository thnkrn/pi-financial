using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Request;

public class HomeMarketInstrumentsRequest
{
    [JsonPropertyName("listName")] public string? ListName { get; set; }

    [JsonPropertyName("relevantTo")] public string? RelevantTo { get; set; }
}