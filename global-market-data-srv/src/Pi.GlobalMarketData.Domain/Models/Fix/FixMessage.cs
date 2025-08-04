using System.Text.Json;

namespace Pi.GlobalMarketData.Domain.Models.Fix;

public class FixMessage
{
    public string? Message { get; set; }
    public string? MessageType { get; set; }

    public static FixMessage? FromJson(string json)
    {
        return JsonSerializer.Deserialize<FixMessage>(json);
    }
}

public class FixData
{
    public string? Symbol { get; set; }
    public string? MDReqID { get; set; }
    public List<Entry>? Entries { get; set; }

    public static FixData? FromJson(string json)
    {
        return JsonSerializer.Deserialize<FixData>(json);
    }
}

public class Entry
{
    public string? MDEntryType { get; set; }
    public double? MDEntryPx { get; set; }
    public double? MDEntrySize { get; set; }
    public DateTime? MDEntryDate { get; set; }
    public DateTime? MDEntryTime { get; set; }
}