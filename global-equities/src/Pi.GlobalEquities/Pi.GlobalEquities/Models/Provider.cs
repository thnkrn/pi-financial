using System.Text.Json.Serialization;

namespace Pi.GlobalEquities.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Provider
{
    Velexa = 1
}
