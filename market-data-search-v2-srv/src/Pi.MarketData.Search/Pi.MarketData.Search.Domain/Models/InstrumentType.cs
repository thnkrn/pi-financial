using System.Text.Json.Serialization;

namespace Pi.MarketData.Search.Domain.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InstrumentType
{
    Unknown = 0,
    Bond,
    Commodity,
    Crypto,
    Currency,
    Derivative,
    Equity,
    Fund,
    GlobalEquity,
    Index,
}

