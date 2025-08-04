using System.Globalization;

namespace Pi.GlobalMarketDataWSS.Application.Helpers;

public static class TimestampExtension
{
    private const string IanaTimeZoneId = "Asia/Bangkok";
    private const string WindowsTimeZoneId = "SE Asia Standard Time";
    private const long SecondsPerMillisecond = 1_000;
    private const string TimeFormat = "HH:mm:ss";

    public static long ToNanosTimestamp(this string? strDatetime)
    {
        if (string.IsNullOrEmpty(strDatetime))
            return 0;

        // If Thai parsing fails, try with invariant culture
        var dateTime = DateTime.Parse(strDatetime, CultureInfo.InvariantCulture);
        var ticksSinceEpoch = (long)(dateTime - DateTime.UnixEpoch).TotalSeconds;

        return ticksSinceEpoch;
    }

    public static long ToUnixMillisecondTimestamp(this string? strDatetime)
    {
        if (string.IsNullOrEmpty(strDatetime))
            return 0;

        var dateTime = DateTime.Parse(strDatetime, CultureInfo.InvariantCulture);
        var ticksSinceEpoch = (long)(dateTime.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds *
                              SecondsPerMillisecond;

        return ticksSinceEpoch;
    }
    public static long ToUnixMillisecondTimestamp(this DateTime dateTime)
    {
        var ticksSinceEpoch = (long)(dateTime.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds *
                              SecondsPerMillisecond;

        return ticksSinceEpoch;
    }

    public static TimeZoneInfo GetThailandTimeZone()
    {
        try
        {
            // Try IANA ID first (for non-Windows systems)
            return TimeZoneInfo.FindSystemTimeZoneById(IanaTimeZoneId);
        }
        catch
        {
            try
            {
                // Fallback to Windows ID
                return TimeZoneInfo.FindSystemTimeZoneById(WindowsTimeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                // If both attempts fail, throw a more informative exception
                throw new TimeZoneNotFoundException(
                    $"Unable to find time zone for Thailand. Neither '{IanaTimeZoneId}' nor '{WindowsTimeZoneId}' were recognized.");
            }
        }
    }

    public static DateTime ConvertToThailandTime(DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Input datetime must be in UTC.", nameof(utcDateTime));

        var thailandTimeZone = GetThailandTimeZone();
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, thailandTimeZone);
    }

    public static string? ConvertToThailandTime(this string utcIsoString)
    {
        try
        {
            // Format strings that match ISO 8601 formats and common variations
            var formats = new[]
            {
                "yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK", // Full precision with timezone
                "yyyy-MM-dd'T'HH:mm:ss.FFFFFFK",
                "yyyy-MM-dd'T'HH:mm:ss.FFFFFK",
                "yyyy-MM-dd'T'HH:mm:ss.FFFFK",
                "yyyy-MM-dd'T'HH:mm:ss.FFFK",
                "yyyy-MM-dd'T'HH:mm:ss.FFK",
                "yyyy-MM-dd'T'HH:mm:ss.FK",
                "yyyy-MM-dd'T'HH:mm:ssK",
                "yyyy-MM-dd'T'HH:mmK",
                "yyyy-MM-dd'T'HH:mm:ss.FFFFFFF", // Without timezone
                "yyyy-MM-dd'T'HH:mm:ss.FFFFFF",
                "yyyy-MM-dd'T'HH:mm:ss.FFFFF",
                "yyyy-MM-dd'T'HH:mm:ss.FFFF",
                "yyyy-MM-dd'T'HH:mm:ss.FFF",
                "yyyy-MM-dd'T'HH:mm:ss.FF",
                "yyyy-MM-dd'T'HH:mm:ss.F",
                "yyyy-MM-dd'T'HH:mm:ss",
                "yyyy-MM-dd'T'HH:mm",
                "yyyy-MM-dd HH:mm:ss.FFFFFFF", // Space instead of T
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd HH:mm",
                "HH:mm:ss.FFFFFFF", // Time only formats
                "HH:mm:ss",
                "HH:mm"
            };

            // First try with UTC adjustment
            if (DateTime.TryParseExact(
                    utcIsoString,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                    out var parsedDate))
            {
                var thailandTime = ConvertToThailandTime(parsedDate);
                return thailandTime.ToString(TimeFormat);
            }

            // If that fails, try parsing as local time
            if (DateTime.TryParseExact(
                    utcIsoString,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out parsedDate))
            {
                // Ensure UTC
                parsedDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
                var thailandTime = ConvertToThailandTime(parsedDate);
                return thailandTime.ToString(TimeFormat);
            }

            // If all parsing attempts fail, try one last time with flexible parsing
            if (DateTime.TryParse(
                    utcIsoString,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                    out parsedDate))
            {
                var thailandTime = ConvertToThailandTime(parsedDate);
                return thailandTime.ToString(TimeFormat);
            }

            return null;
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            return null;
        }
    }

    public static string ExtractTimeFromDateTime(this string dateTimeStr)
    {
        // Create a Thai culture info
        var thaiCulture = new CultureInfo("th-TH")
        {
            DateTimeFormat =
            {
                Calendar = new ThaiBuddhistCalendar()
            }
        };

        // Define GMT+7 timezone
        var bangkokTimeZone = TimeZoneInfo.CreateCustomTimeZone(
            "GMT+7",
            new TimeSpan(7, 0, 0),
            "GMT+7",
            "GMT+7"
        );

        // Try parsing with Thai Buddhist calendar
        if (DateTime.TryParse(dateTimeStr, thaiCulture, DateTimeStyles.None, out var dateTime))
        {
            // Convert UTC to GMT+7
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
                bangkokTimeZone
            );
            return dateTime.ToString(TimeFormat);
        }

        // If Thai parsing fails, parse with invariant culture
        dateTime = DateTime.Parse(dateTimeStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

        // Convert UTC to GMT+7
        dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, bangkokTimeZone);

        // Return the time part formatted as "HH:mm:ss"
        return dateTime.ToString(TimeFormat);
    }
}