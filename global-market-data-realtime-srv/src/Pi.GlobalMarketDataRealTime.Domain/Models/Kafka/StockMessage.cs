using Newtonsoft.Json;

namespace Pi.GlobalMarketDataRealTime.Domain.Models.Kafka;

// ReSharper disable UnusedAutoPropertyAccessor.Global
public class StockMessage
{
    public string? Message { get; set; }

    public string? MessageType { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}