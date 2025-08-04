using Newtonsoft.Json;

namespace Pi.GlobalMarketDataRealTime.Domain.Models.Fix;

public class MarketDataEntry
{
    [JsonProperty("MDEntryType")] 
    public string? MdEntryType { get; set; }

    [JsonProperty("MDEntryPx")] 
    public decimal MdEntryPx { get; set; }

    [JsonProperty("MDEntrySize")] 
    public decimal MdEntrySize { get; set; }

    [JsonProperty("MDEntryDate")] 
    public DateTime? MdEntryDate { get; set; }

    [JsonProperty("MDEntryTime")] 
    public DateTime? MdEntryTime { get; set; }
}

public class MarketDataSnapshot
{
    [JsonProperty(nameof(Symbol))] 
    public string? Symbol { get; set; }

    [JsonProperty(nameof(SendingTime))] 
    public string? SendingTime { get; set; }

    [JsonProperty(nameof(SequenceNumber))] 
    public long? SequenceNumber { get; set; }

    [JsonProperty("MDReqID")] 
    public string? MdReqId { get; set; }

    [JsonProperty("MDEntryType")] 
    public string? MdEntryType { get; set; }

    [JsonProperty(nameof(Entries))] 
    public List<MarketDataEntry> Entries { get; set; } = [];
}