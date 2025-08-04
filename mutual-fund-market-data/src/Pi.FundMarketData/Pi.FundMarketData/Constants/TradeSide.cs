using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Pi.FundMarketData.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TradeSide
{
    Buy = 1,
    Sell,
    Switch
}
