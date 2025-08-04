using Newtonsoft.Json;

namespace Pi.SetMarketData.MigrationProxy.Models;

public class RequestPayload
{
    [JsonProperty("data")]
    public Data? Data { get; set; }
    [JsonProperty("op")]
    public string? Op { get; set; }
    [JsonProperty("sessionId")]
    public string? SessionId { get; set; }
}

public class Data
{
    [JsonProperty("param")]
    public List<Parameter>? Param { get; set; }
    [JsonProperty("subscribeType")]
    public string? SubscribeType { get; set; }
}

public class Parameter
{
    [JsonProperty("market")]
    public string? Market { get; set; }
    [JsonProperty("symbol")]
    public string? Symbol { get; set; }
}