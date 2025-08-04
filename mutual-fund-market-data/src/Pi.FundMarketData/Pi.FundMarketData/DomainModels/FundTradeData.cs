using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.DomainModels;

public class FundTradeData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; init; }
    public string Symbol { get; init; }
    public List<TradeCalendar> TradeCalendars { get; init; } = new();
    public List<Switching> Switchings { get; init; } = new();
    public List<DateTime> Holidays { get; init; } = new();
    public DateTime LastModified { get; init; }

    public IEnumerable<DateTime> GetTradableDates(
       TradeSide tradeSide,
       TradeCalendarType? tradeCalendarType,
       IEnumerable<DateTime> businessDays)
    {
        var tradableDates = Enumerable.Empty<DateTime>();

        switch (tradeCalendarType)
        {
            // business days - fund holidays
            case TradeCalendarType.BusinessDay:
                tradableDates = businessDays.Except(Holidays);
                break;
            // business days + allowed days
            case TradeCalendarType.FocusAllowedDay:
                {
                    var allowedDates = FilterTradeCalendars(tradeSide, TradePermission.Allowed, TradeCalendars);
                    tradableDates = allowedDates
                        .Intersect(businessDays)
                        .Except(Holidays);
                    break;
                }
            // business days - fund holidays - disallowed days
            case TradeCalendarType.ExcludeDisallowedDay:
                {
                    var disallowedDates = FilterTradeCalendars(tradeSide, TradePermission.Disallowed, TradeCalendars);
                    tradableDates = businessDays
                        .Except(disallowedDates)
                        .Except(Holidays);
                    break;
                }
        }

        return tradableDates;
    }

    private static IEnumerable<DateTime> FilterTradeCalendars(
        TradeSide type,
        string tradePermission,
        IEnumerable<TradeCalendar> dates) =>
        type switch
        {
            TradeSide.Buy => dates.Where(x => x.TransactionCode == "SUB" && x.TradePermission == tradePermission).Select(x => x.TradeDate),
            TradeSide.Sell => dates.Where(x => x.TransactionCode == "RED" && x.TradePermission == tradePermission).Select(x => x.TradeDate),
            TradeSide.Switch => dates.Where(x => x.TransactionCode == "SWO" && x.TradePermission == tradePermission).Select(x => x.TradeDate),
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected TradeType value: {type}.")
        };
}
