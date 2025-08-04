using System.Globalization;

namespace Pi.FundMarketData.Utils;

public static class DateTimeUtils
{
    public static string ConvertToString(this DateTime date)
    {
        return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    public static bool IsWeekend(this DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }
}
