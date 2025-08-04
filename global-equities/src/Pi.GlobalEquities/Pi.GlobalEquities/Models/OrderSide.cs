using System.Text.Json.Serialization;

namespace Pi.GlobalEquities.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderSide
{
    Buy = 1,
    Sell
}
