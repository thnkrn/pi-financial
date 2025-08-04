namespace Pi.GlobalMarketData.Domain.ConstantConfigurations;

public static class MarketTime
{
    public static readonly Dictionary<string, string> StartTime = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "ARCA", "20:30:00" },
        { "BATS", "20:30:00" },
        { "HKEX", "08:30:00" },
        { "NASDAQ", "20:30:00" },
        { "NYSE", "20:30:00" }
    };
}
