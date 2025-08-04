namespace Pi.MarketData.MigrationProxy.API.Configurations;

public static class MarketStatusFilter
{
    public static readonly HashSet<string> SetMarket = ["SET", "mai"];
    public static readonly HashSet<string> GeMarket = ["NASDAQ", "HK"];
}