namespace Pi.SetMarketData.Infrastructure.Handlers.MarketTimelineRendered;

public class InstrumentMarketMapper
{
    private static readonly Dictionary<string, string> MarketToIntermissionType = new()
    {
        ["SET"] = "Thai Equity",
        ["MAI"] = "Thai Equity",
        ["TXI"] = "Derivative Day",
        ["TXS"] = "Derivative Day",
        ["TXA"] = "",
        // Default will be "Derivative Night"
    };

    public static string GetIntermissionType(string market) =>
        MarketToIntermissionType.GetValueOrDefault(market, "Derivative Night");
}
