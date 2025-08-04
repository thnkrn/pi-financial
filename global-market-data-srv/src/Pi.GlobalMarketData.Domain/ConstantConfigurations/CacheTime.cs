namespace Pi.GlobalMarketData.Domain.ConstantConfigurations;

public static class CacheTime
{
    public static readonly TimeSpan CacheMinute = new TimeSpan(0, 0, 1, 0);
    public static readonly TimeSpan CacheHour = new TimeSpan(0, 1, 0, 0);
    public static readonly TimeSpan CacheDay = new TimeSpan(1, 0, 0, 0);
    public static readonly TimeSpan CacheWeek = new TimeSpan(7, 0, 0, 0);
}