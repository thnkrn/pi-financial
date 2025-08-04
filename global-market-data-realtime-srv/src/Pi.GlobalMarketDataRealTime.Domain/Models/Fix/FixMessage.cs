using Newtonsoft.Json;

namespace Pi.GlobalMarketDataRealTime.Domain.Models.Fix;

public class FixMessage
{
    [JsonProperty(nameof(Message))] public string? Message { get; set; }

    [JsonProperty(nameof(MessageType))] public string? MessageType { get; set; }

    public static FixMessage? FromJson(string json)
    {
        return JsonConvert.DeserializeObject<FixMessage>(json);
    }
}

public class FixData
{
    [JsonProperty(nameof(Symbol))] public string? Symbol { get; set; }

    [JsonProperty("MDReqID")] public string? MdReqId { get; set; }

    [JsonProperty(nameof(Entries))] public List<Entry>? Entries { get; set; }

    public static FixData? FromJson(string json)
    {
        return JsonConvert.DeserializeObject<FixData>(json);
    }
}

public class Entry
{
    [JsonProperty("MDEntryType")] public string? MdEntryType { get; set; }

    [JsonProperty("MDEntryPx")] public double? MdEntryPx { get; set; }

    [JsonProperty("MDEntrySize")] public double? MdEntrySize { get; set; }

    [JsonProperty("MDEntryDate")] public DateTime? MdEntryDate { get; set; }

    [JsonProperty("MDEntryTime")] public DateTime? MdEntryTime { get; set; }
}