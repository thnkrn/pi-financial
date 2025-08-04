using System.Text.Json.Serialization;

namespace Pi.GlobalEquities.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderType
{
    Unknown = -1,
    Market = 1,
    Limit,
    StopLimit,
    Stop,
    Twap,
    TrailingStop,
    Iceberg,
    Pov,
    Vwap,
    Tpsl,
    TakeProfit,
    StopLoss
}
