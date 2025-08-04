using System.Text.Json.Serialization;

namespace Pi.GlobalEquities.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Unknown = -1,
    Queued = 1,
    Processing,
    PartiallyMatched,
    Matched,
    Cancelled,
    Rejected
}
