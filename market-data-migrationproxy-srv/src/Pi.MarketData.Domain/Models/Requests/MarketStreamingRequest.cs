using System.Text.Json.Serialization;

namespace Pi.MarketDataWSS.Domain.Models.Request;

public class Data
{
    [JsonPropertyName("param")] public List<MarketStreamingParameter>? Param { get; set; }

    [JsonPropertyName("subscribeType")] public string? SubscribeType { get; set; }
}

public class MarketStreamingParameter
{
    [JsonPropertyName("market")] public string? Market { get; set; }

    [JsonPropertyName("symbol")] public string? Symbol { get; set; }
}

public class MarketStreamingRequest
{
    [JsonPropertyName("data")] public Data? Data { get; set; }

    [JsonPropertyName("op")] public string? Op { get; set; }

    [JsonPropertyName("sessionId")] public string? SessionId { get; set; }
}