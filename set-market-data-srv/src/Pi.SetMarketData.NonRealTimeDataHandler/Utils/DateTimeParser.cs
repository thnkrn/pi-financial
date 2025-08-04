using System.Globalization;
using Pi.SetMarketData.NonRealTimeDataHandler.constants;

namespace Pi.SetMarketData.NonRealTimeDataHandler.Utils;

public class DateTimeParser
{
    public static DateTime Parse(string dateTimeString)
    {
        CultureInfo enUS = new("en-US");

        if (
            DateTime.TryParseExact(
                dateTimeString,
                DateTimeFormatPatterns.MorningStarDateTimeFormatLong,
                enUS,
                DateTimeStyles.None,
                out DateTime dateValue
            )
        )
            return dateValue;

        if (
            DateTime.TryParseExact(
                dateTimeString,
                DateTimeFormatPatterns.MorningStarDateTimeFormatShort,
                enUS,
                DateTimeStyles.None,
                out dateValue
            )
        )
            return dateValue;

        if (
            DateTime.TryParseExact(
                dateTimeString,
                DateTimeFormatPatterns.MorningStarDateTimeFormat6Digit,
                enUS,
                DateTimeStyles.None,
                out dateValue
            )
        )
            return dateValue;

        if (
            DateTime.TryParseExact(
                dateTimeString,
                DateTimeFormatPatterns.MorningStarDateTimeFormat3Digit,
                enUS,
                DateTimeStyles.None,
                out dateValue
            )
        )
            return dateValue;

        throw new Exception("Incorrect date format.");
    }
}
