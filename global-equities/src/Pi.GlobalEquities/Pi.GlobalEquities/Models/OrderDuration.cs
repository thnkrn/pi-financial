using System.Text.Json.Serialization;

namespace Pi.GlobalEquities.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderDuration
{
    Unknown = -1,
    Day = 1,
    Fok,
    Ioc,
    Gtc,
    Gtt,
    Ato,
    Atc
}
