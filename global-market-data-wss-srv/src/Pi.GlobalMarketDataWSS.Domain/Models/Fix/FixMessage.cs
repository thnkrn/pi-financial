using Newtonsoft.Json;

namespace Pi.GlobalMarketDataWSS.Domain.Models.Fix;

/// <summary>
///     Represents a FIX (Financial Information eXchange) message.
/// </summary>
public class FixMessage
{
    [JsonProperty(nameof(Message))] public string? Message { get; set; }

    [JsonProperty(nameof(MessageType))] public string? MessageType { get; set; }

    /// <summary>
    ///     Deserializes a JSON string to a FixMessage object.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>A FixMessage object, or null if deserialization fails.</returns>
    public static FixMessage? FromJson(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<FixMessage>(json);
        }
        catch (JsonException)
        {
            // Log the exception here if needed
            return null;
        }
    }

    /// <summary>
    ///     Serializes the FixMessage object to a JSON string.
    /// </summary>
    /// <returns>A JSON string representation of the FixMessage object.</returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

/// <summary>
///     Represents FIX data containing symbol information and entries.
/// </summary>
public class FixData
{
    [JsonProperty(nameof(Symbol))] public string? Symbol { get; set; }

    [JsonProperty(nameof(SendingTime))] public string? SendingTime { get; set; }
    
    [JsonProperty(nameof(SequenceNumber))] public long? SequenceNumber { get; set; }

    [JsonProperty("MDReqID")] public string? MdReqId { get; set; }
    [JsonProperty("MDEntryType")] public string? MdEntryType { get; set; }

    [JsonProperty(nameof(Entries))] public List<Entry>? Entries { get; set; }

    /// <summary>
    ///     Deserializes a JSON string to a FixData object.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>A FixData object, or null if deserialization fails.</returns>
    public static FixData? FromJson(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<FixData>(json);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            // Log the exception here if needed
            return null;
        }
    }

    /// <summary>
    ///     Serializes the FixData object to a JSON string.
    /// </summary>
    /// <returns>A JSON string representation of the FixData object.</returns>
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class Entry
{
    [JsonProperty("MDEntryType")] public string? MdEntryType { get; set; }

    [JsonProperty("MDEntryPx")] public decimal MdEntryPx { get; set; }

    [JsonProperty("MDEntrySize")] public decimal? MdEntrySize { get; set; }

    [JsonProperty("MDEntryDate")] public DateTime MdEntryDate { get; set; }

    [JsonProperty("MDEntryTime")] public DateTime MdEntryTime { get; set; }
}