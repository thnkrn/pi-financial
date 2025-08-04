using System.Text.Json;

namespace Pi.SetMarketDataWSS.Domain.Models.Kafka;

public class StockMessage
{
    public string? Message { get; set; }
    public string? MessageType { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}