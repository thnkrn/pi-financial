namespace Pi.SetMarketData.Domain.ConstantConfigurations;

public static class MarketTime
{
    public static readonly Dictionary<string, string> StartTime = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "Equity", "08:00:00" },
        { "Derivative", "08:00:00" },
    };
}
