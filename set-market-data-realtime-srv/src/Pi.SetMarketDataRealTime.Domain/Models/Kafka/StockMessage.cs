using System.Text.Json;

namespace Pi.SetMarketDataRealTime.Domain.Models.Kafka;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class StockMessage
{
    public string? Message { get; set; }
    public string? MessageType { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}