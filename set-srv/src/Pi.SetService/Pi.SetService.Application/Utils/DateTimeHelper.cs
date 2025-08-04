namespace Pi.SetService.Application.Utils;

public static class DateTimeHelper
{
    public static string ThTimeZone => "Asia/Bangkok";
    public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    public const string DateFormat = "yyyy-MM-dd";

    public static DateTime ThNow()
    {
        return ConvertThTimeFromUtc(DateTime.UtcNow);
    }

    public static DateTime ConvertThTimeToUtc(DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(ThTimeZone));
    }

    public static DateTime ConvertThTimeFromUtc(DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(ThTimeZone));
    }

    public static long ToUnixTime(DateTime dateTime)
    {
        try
        {
            var localDateTimeOffset = new DateTimeOffset(dateTime);
            return localDateTimeOffset.ToUnixTimeMilliseconds();
        }
        catch (Exception ex)
        {
            throw new ArgumentNullException(ex.Message);
        }
    }
}
