namespace Pi.GlobalMarketDataWSS.Application.Services.Constants;

public static class CacheKey
{
    public const string StreamingBody = "global-streaming-body-";
    public const string GeInstrument = "global-instrument-";
    public const string Whitelist = "whitelist-data-";
    public const string GeResetStatsTracking = "reset-stats-tracking-";
    public const string GeMarketSchedule = "ge-market-schedule-list-";
    public const string PriceStreamingBody = "{ge-str-streaming-body-}";
    public const string PriorClose = "prior-close-";
    public const string Open = "open-";
}