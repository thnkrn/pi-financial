using System.Text.Json.Serialization;

namespace Pi.FundMarketData.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TradeCalendarType
{
    BusinessDay = 1,
    FocusAllowedDay = 2,
    ExcludeDisallowedDay = 3
}

public static class TradeCalendarTypeExtension
{
    public static TradeCalendarType? ToTradeCalendarType(this string shortCode)
    {
        return shortCode switch
        {
            "N" => TradeCalendarType.BusinessDay,
            "E" => TradeCalendarType.ExcludeDisallowedDay,
            "O" => TradeCalendarType.FocusAllowedDay,
            _ => null
        };
    }
}

