using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Pi.FundMarketData.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MarketBasket
{
    TopFund = 1,
    Category,
    NewFund,
}
