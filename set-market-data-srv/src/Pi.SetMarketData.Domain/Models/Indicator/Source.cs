using Newtonsoft.Json;

namespace Pi.SetMarketData.Domain.Models.Indicator;

// TODO: Remove unused properties
public partial class Source
{
    [JsonProperty("version")]
    public string? Version { get; set; }

    [JsonProperty("connector")]
    public string? Connector { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("ts_ms")]
    public long TsMs { get; set; }

    [JsonProperty("snapshot")]
    public string? Snapshot { get; set; }

    [JsonProperty("db")]
    public string? Db { get; set; }

    [JsonProperty("sequence")]
    public string? Sequence { get; set; }

    [JsonProperty("ts_us")]
    public long TsUs { get; set; }

    [JsonProperty("ts_ns")]
    public double TsNs { get; set; }

    [JsonProperty("schema")]
    public string? Schema { get; set; }

    [JsonProperty("table")]
    public string? Table { get; set; }

    [JsonProperty("txId")]
    public long TxId { get; set; }

    [JsonProperty("lsn")]
    public long Lsn { get; set; }

    [JsonProperty("xmin")]
    public object? Xmin { get; set; }
}