using Confluent.Kafka;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Models;

public class VariableWrapper
{
    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    public string? Symbol { get; set; }
    public string? Venue { get; set; }
    public string? MarketSession { get; set; }
    public string? SendingTime { get; set; }
    public long? SequenceNumber { get; set; }
    public string? MdEntryType { get; set; }
    public Timestamp CreationTime { get; set; }
}