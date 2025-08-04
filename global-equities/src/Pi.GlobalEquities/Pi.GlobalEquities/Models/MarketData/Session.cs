using System.Text.Json.Serialization;

namespace Pi.GlobalEquities.Models.MarketData;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Session
{
    Unknown = -1,
    PreMarket = 1,
    MainSession,
    AfterMarket,
    Clearing,
    Offline,
    Online,
    Expired
}
