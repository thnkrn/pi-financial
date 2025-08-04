using Newtonsoft.Json;

namespace Pi.SetMarketData.Domain.Models.Indicator;

// TODO: Remove unused properties
public partial class CandleEventMessage
{
    // [JsonProperty("before")]
    // public object? Before { get; set; }

    [JsonProperty("after")]
    public After? After { get; set; }

    // [JsonProperty("source")]
    // public Source? Source { get; set; }

    // [JsonProperty("transaction")]
    // public object? Transaction { get; set; }

    // [JsonProperty("op")]
    // public string? Op { get; set; }

    // [JsonProperty("ts_ms")]
    // public long TsMs { get; set; }

    // [JsonProperty("ts_us")]
    // public long TsUs { get; set; }

    // [JsonProperty("ts_ns")]
    // public double TsNs { get; set; }
}