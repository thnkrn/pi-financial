namespace Pi.FundMarketData.Repositories;

public class DataCacheConfig
{
    public TimeSpan AmcProfileCacheDuration { get; init; }
    public TimeSpan LatestNavCacheDuration { get; init; }
}
