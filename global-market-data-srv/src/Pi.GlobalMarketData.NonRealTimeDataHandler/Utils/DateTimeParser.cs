using System.Globalization;
using Pi.GlobalMarketData.NonRealTimeDataHandler.constants;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Utils;

public class DateTimeParser
{
    public static DateTime Parse(string dateTimeString)
    {
        CultureInfo enUS = new("en-US");

        if (
            DateTime.TryParseExact(
                dateTimeString,
                DateTimeFormatPatterns.MorningstarDateTimeFormatLong,
                enUS,
                DateTimeStyles.None,
                out var dateValue
            )
        )
            return dateValue;

        if (
            DateTime.TryParseExact(
                dateTimeString,
                DateTimeFormatPatterns.MorningstarDateTimeFormatShort,
                enUS,
                DateTimeStyles.None,
                out dateValue
            )
        )
            return dateValue;

        if (
            DateTime.TryParseExact(
                dateTimeString,
                DateTimeFormatPatterns.MorningstarDateTimeFormat6Digit,
                enUS,
                DateTimeStyles.None,
                out dateValue
            )
        )
            return dateValue;

        if (
            DateTime.TryParseExact(
                dateTimeString,
                DateTimeFormatPatterns.MorningstarDateTimeFormat3Digit,
                enUS,
                DateTimeStyles.None,
                out dateValue
            )
        )
            return dateValue;

        throw new Exception("Incorrect date format.");
    }
}