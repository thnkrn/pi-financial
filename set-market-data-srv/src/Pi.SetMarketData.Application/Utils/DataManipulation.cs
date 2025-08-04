using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NodaTime;
using Pi.SetMarketData.Application.Constants;

namespace Pi.SetMarketData.Application.Utils;

public static class DataManipulation
{
    private const string DefaultValue = "0.00";
    private static readonly string[] Formats =
    {
        "dd/MM/yyyy HH:mm:ss",
        "dd/MM/yyyy",
    };

    private static readonly Dictionary<string, string> CountryCodeToNameMap = new Dictionary<
        string,
        string
    >
    {
        { "THA", "Thailand" },
        { "USA", "United States" },
        { "CAN", "Canada" },
        { "GBR", "United Kingdom" },
        { "FRA", "France" },
        { "DEU", "Germany" },
        { "JPN", "Japan" },
        { "CHN", "China" },
        { "IND", "India" },
        { "BRA", "Brazil" },
        { "AUS", "Australia" },
        { "KOR", "South Korea" },
        { "MEX", "Mexico" },
        { "IDN", "Indonesia" },
        { "RUS", "Russia" },
        { "TUR", "Turkey" },
        { "ZAF", "South Africa" },
        { "SGP", "Singapore" },
        { "HKG", "Hong Kong" },
        { "MYS", "Malaysia" },
        { "PHL", "Philippines" },
        { "VNM", "Vietnam" },
        { "NZL", "New Zealand" },
        { "PAK", "Pakistan" },
        { "EGY", "Egypt" },
        { "ARG", "Argentina" },
        { "PER", "Peru" },
        { "CHL", "Chile" },
        { "COL", "Colombia" },
        { "SAU", "Saudi Arabia" },
        { "ARE", "United Arab Emirates" },
        { "QAT", "Qatar" },
        { "KWT", "Kuwait" },
        { "OMN", "Oman" },
    };

    public static string ToYy(double oldData, double newData)
    {
        if (oldData == 0 && newData == 0)
            return "0.00";

        if (oldData == 0)
            return newData > 0 ? "100.00" : "-100.00";

        if (newData == 0)
            return oldData > 0 ? "-100.00" : "100.00";

        double yy = (newData - oldData) / Math.Abs(oldData) * 100;

        if (double.IsNaN(yy) || double.IsInfinity(yy))
            return "0.00";

        return yy.ToString(DataFormat.TwoDigitFormat);
    }

    public static decimal? RoundNumber(double? value, int digit = 2)
    {
        if (value == null)
            return null;

        return Math.Round((decimal)value, digit);
    }

    public static string RemoveTrailingZeros(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "0";
        if (double.TryParse(input, out double number))
        {
            return number.ToString("G");
        }
        else
        {
            throw new ArgumentException("Invalid input format.");
        }
    }

    public static List<string> ToListString(double[] data)
    {
        List<string> list = [];

        if (data == null || data.Length == 0)
            return list;

        list = data.Select(x => x.ToString("0.##########")).ToList();

        return list;
    }

    public static string? DateToString(DateTime dateTime, string? format = null)
    {
        if (dateTime == DateTime.MinValue)
            return "";

        var dateInThailand = TimeZoneInfo.ConvertTimeFromUtc(
            dateTime.ToUniversalTime(),
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok")
        );
        return dateInThailand.ToString(
            format ?? DataFormat.YearMonthDayFormat,
            CultureInfo.InvariantCulture
        );
    }

    public static string GetCountryName(string countryCode)
    {
        if (CountryCodeToNameMap.TryGetValue(countryCode, out var countryName))
        {
            return countryName;
        }
        return "";
    }

    public static string FormatDouble(double value)
    {
        // Use custom format string "N0" if value is an integer, otherwise "N"
        return value % 1 == 0
            ? value.ToString("N0")
            : value.ToString("N").TrimEnd('0').TrimEnd('.');
    }

    public static string FormatDecimals(
        this string? value,
        int decimals,
        bool handleUnavailableValue = false,
        int fixedDecimal = 0
    )
    {
        // Handle the case where value is null, empty or "0" then return "0.00"
        if (string.IsNullOrEmpty(value) || value == "0")
            return DefaultValue;

        if (handleUnavailableValue)
            value = HandleUnavailableValue(value);

        // Handle the case where decimals is 0
        if (decimals == 0 && value.Contains('.'))
            // If value contains a decimal point, return it unchanged
            return value;

        // Remove any existing decimal point
        value = value.Replace(".", "");

        // Calculate decimal point
        _ = double.TryParse(value, out var decimalsInInteger);
        var formattedValue = decimalsInInteger / Math.Pow(10, decimals);
        var result = formattedValue.ToString("0.00######");

        // Formatting result
        if (fixedDecimal > 0)
            result = formattedValue.ToString($"F{fixedDecimal}");

        return result;
    }

    public static long ToUnixTimestamp(DateTime dt)
    {
        var dateTimeOffset = new DateTimeOffset(dt);
        return dateTimeOffset.ToUnixTimeSeconds();
    }

    public static long ToUnixTimestampMilliseconds(DateTime dt)
    {
        var dateTimeOffset = new DateTimeOffset(dt);
        return dateTimeOffset.ToUnixTimeMilliseconds();
    }

    public static DateTime ToUTCDateTime(int unixTimeSeconds)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds);
        return dateTimeOffset.UtcDateTime;
    }

    public static int GetOffset(string timezone)
    {
        try
        {
            if (string.IsNullOrEmpty(timezone))
                return 0;
            TimeZoneInfo exchangeTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            DateTime currentTime = DateTime.UtcNow;
            TimeSpan offset = exchangeTimeZone.GetUtcOffset(currentTime);

            return (int)offset.TotalSeconds;
        }
        catch (TimeZoneNotFoundException)
        {
            DateTimeZone exchangeTimeZone = DateTimeZoneProviders.Tzdb[timezone];
            Instant currentTime = SystemClock.Instance.GetCurrentInstant();

            ZonedDateTime time = currentTime.InZone(exchangeTimeZone);
            return time.Offset.Seconds;
        }
        catch (InvalidTimeZoneException error)
        {
            throw new InvalidTimeZoneException("Invalid timezone", error);
        }
    }

    public static string ToJsonString(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public static string ToMonthYear(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return TryParseDateTime(value, out DateTime date)
            ? date.ToString("MMM yy", CultureInfo.InvariantCulture)
            : string.Empty;
    }

    public static string ToDayMonthYear(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return TryParseDateTime(value, out DateTime date)
            ? date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)
            : string.Empty;
    }

    public static string CalculateDaysUntilDate(string dateString)
    {
        if (string.IsNullOrEmpty(dateString))
            return string.Empty;

        try
        {
            if (!TryParseDateTime(dateString, out DateTime date))
                return string.Empty;

            var daysUntil = (date.Date - DateTime.Today).Days + 1;
            return daysUntil <= 0 ? "" : daysUntil.ToString();
        }
        catch (FormatException)
        {
            return string.Empty;
        }
    }

    private static string HandleUnavailableValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return DefaultValue;

        var result = value.Equals("-2147483648") ? DefaultValue : value;
        return result;
    }

    public static string GetMonthNameAbbreviation(int monthNumber)
    {
        if (monthNumber < 1 || monthNumber > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(monthNumber),
                "Month number must be between 1 and 12."
            );
        }

        switch (monthNumber)
        {
            case 1:
                return "Jan";
            case 2:
                return "Feb";
            case 3:
                return "Mar";
            case 4:
                return "Apr";
            case 5:
                return "May";
            case 6:
                return "Jun";
            case 7:
                return "Jul";
            case 8:
                return "Aug";
            case 9:
                return "Sep";
            case 10:
                return "Oct";
            case 11:
                return "Nov";
            case 12:
                return "Dec";
            default:
                throw new InvalidOperationException("Unexpected month number.");
        }
    }

    public static string RemoveSpace(string value) =>
        Regex.Replace(
            value ?? string.Empty,
            @"\s+",
            string.Empty,
            RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(3)
        );

    public static bool TryParseDateTime(string? value, out DateTime result)
    {
        if (string.IsNullOrEmpty(value))
        {
            result = default;
            return false;
        }

        // First try explicit formats
        if (DateTime.TryParseExact(
            value,
            Formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out result))
        {
            return true;
        }

        // Then try general parsing
        return DateTime.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out result);
    }
}
