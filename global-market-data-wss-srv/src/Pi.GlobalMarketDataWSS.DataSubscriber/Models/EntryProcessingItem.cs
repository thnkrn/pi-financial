using Confluent.Kafka;
using Pi.GlobalMarketDataWSS.Domain.Models.Fix;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Models;

public class EntryProcessingItem
{
    public required Entry Entry { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public string? SendingTime { get; set; }
    public long? SequenceNumber { get; set; }
    public string? MdEntryType { get; set; }
    public Timestamp CreationTime { get; set; }
}