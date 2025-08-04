namespace Pi.WalletService.Application.Utilities;

public static class DateUtils
{
    private static readonly TimeZoneInfo ThTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
    public static DateTime GetThDateTimeNow() => TimeZoneInfo.ConvertTime(DateTime.UtcNow, ThTimeZoneInfo);

    public static DateTime GetThDateTimeFromUtc(DateTime thDateTime) =>
        TimeZoneInfo.ConvertTime(thDateTime, ThTimeZoneInfo);

    public static DateTime GetThDateTime(DateOnly date, TimeOnly time)
    {
        return new DateTimeOffset(
            date.Year,
            date.Month,
            date.Day,
            time.Hour,
            time.Minute,
            time.Second,
            time.Millisecond,
            time.Microsecond,
            ThTimeZoneInfo.BaseUtcOffset
        ).DateTime;
    }
    public static (DateTime startDateTime, DateTime endDateTime) GetUtcStartEndDateTime(DateOnly date, TimeOnly cutOffTime, bool isThTime)
    {
        var startDateTime = date.ToDateTime(cutOffTime, DateTimeKind.Utc);
        var endDateTime = date.AddDays(1).ToDateTime(cutOffTime, DateTimeKind.Utc).AddTicks(-1);

        // ReSharper disable once InvertIf doesnt make sense
        if (isThTime)
        {
            var offset = ThTimeZoneInfo.BaseUtcOffset;
            startDateTime = startDateTime.Add(-offset);
            endDateTime = endDateTime.Add(-offset);
        }

        return (startDateTime, endDateTime);
    }
}