using Newtonsoft.Json;

namespace Pi.MarketData.Domain.Models.Response;

public class ResponseWrapper
{
    [JsonProperty("set")]
    public object? Set { get; set; }

    [JsonProperty("ge")]
    public object? Ge { get; set; }
}