namespace Pi.MarketData.Search.Domain.ConstantConfigurations;

public static class DomainVenue
{
    public static readonly HashSet<string> SetVenue = ["Equity"];
    public static readonly HashSet<string> GeVenue = ["ARCA", "BATS", "HKEX", "NASDAQ", "NYSE"];
    public static readonly HashSet<string> TfexVenue = ["Derivative"];
}