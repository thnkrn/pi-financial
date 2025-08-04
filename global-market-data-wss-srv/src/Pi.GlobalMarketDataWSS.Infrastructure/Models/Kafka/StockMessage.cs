using System.Text.Json;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Models.Kafka;

public class StockMessage
{
    public string? Message { get; set; }
    public string? MessageType { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}