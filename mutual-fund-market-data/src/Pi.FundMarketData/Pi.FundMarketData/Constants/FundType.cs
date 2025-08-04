namespace Pi.FundMarketData.Constants;

public enum FundType
{
    Unknown = 0,
    Plain,
    Complex
}

public static class FundTypeExtension
{
    public static FundType ParseFundType(string value)
    {
        return value switch
        {
            "P" => FundType.Plain,
            "C" => FundType.Complex,
            _ => FundType.Unknown
        };
    }
}
