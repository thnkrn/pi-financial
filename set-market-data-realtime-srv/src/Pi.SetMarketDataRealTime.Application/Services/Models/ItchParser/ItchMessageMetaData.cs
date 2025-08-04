namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class ItchMessageMetadata
{
    public long Timestamp { get; set; }
    public string? Session { get; set; }
    public ulong SequenceNumber { get; set; }

    public string? OrderBookId { get; set; }
}