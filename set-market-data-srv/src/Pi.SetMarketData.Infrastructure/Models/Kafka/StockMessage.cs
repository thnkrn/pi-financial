namespace Pi.SetMarketData.Infrastructure.Models.Kafka;

public class StockMessage
{
    public string? Message { get; set; }
    public string? MessageType { get; set; }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}