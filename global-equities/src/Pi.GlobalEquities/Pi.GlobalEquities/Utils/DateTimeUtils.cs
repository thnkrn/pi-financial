namespace Pi.GlobalEquities.Utils;

public static class DateTimeUtils
{
    public static long ConvertToTimestamp(DateTime dateTime)
    {
        var unixTime = ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
        return unixTime;
    }

    public static DateTime ConvertToDateTimeUtc(long unixTime)
    {
        var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(unixTime).UtcDateTime;
        return dateTime;
    }

    public static DateTime ConvertToDateTimeUs(DateTime dateTimeUtc)
    {
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        return TimeZoneInfo.ConvertTime(dateTimeUtc, TimeZoneInfo.Utc, easternTimeZone);
    }

    public static DateTime ConvertToDateTimeHk(DateTime dateTimeUtc)
    {
        var chinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
        return TimeZoneInfo.ConvertTime(dateTimeUtc, TimeZoneInfo.Utc, chinaTimeZone);
    }
}
