namespace Pi.Financial.FundService.Application.Utils;

public static class DateUtils
{
    private static readonly TimeZoneInfo ThTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
    public static DateTime GetThDateTimeNow() => TimeZoneInfo.ConvertTime(DateTime.UtcNow, ThTimeZoneInfo);
    public static DateOnly GetThToday() => DateOnly.FromDateTime(GetThDateTimeNow());
}
