using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Pi.FundMarketData.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Interval
{
    [EnumMember(Value = "3M")] Over3Months = 1,

    [EnumMember(Value = "6M")] Over6Months,

    [EnumMember(Value = "1Y")] Over1Year,

    [EnumMember(Value = "3Y")] Over3Years,

    [EnumMember(Value = "5Y")] Over5Years,

    [EnumMember(Value = "YTD")] YearToDate,

    [EnumMember(Value = "SI")] SinceInception
}

public static class IntervalExtension
{
    public static (DateTime start, DateTime end) GetIntervalDateTimes(this Interval interval, DateTime? beginDate = null)
    {
        DateTime end = beginDate ?? DateTime.UtcNow.Date;
        DateTime start = interval switch
        {
            Interval.Over3Months => end.AddMonths(-3),
            Interval.Over6Months => end.AddMonths(-6),
            Interval.Over1Year => end.AddYears(-1),
            Interval.Over3Years => end.AddYears(-3),
            Interval.Over5Years => end.AddYears(-5),
            Interval.YearToDate => new DateTime(end.Year, 1, 1),
            Interval.SinceInception => DateTime.MinValue,
            _ => throw new ArgumentOutOfRangeException(nameof(interval), $"Not expected Interval value: {interval}.")
        };

        return (start, end);
    }
}
