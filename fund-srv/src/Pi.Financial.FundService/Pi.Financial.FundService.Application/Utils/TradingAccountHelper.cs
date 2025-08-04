namespace Pi.Financial.FundService.Application.Utils;

public static class TradingAccountHelper
{
    public static string? GetCustCodeByFundTradingAccountNo(string tradingAccountNo)
    {
        if (!tradingAccountNo.EndsWith("-1") && !tradingAccountNo.ToLower().EndsWith("-m")) return null;

        const int suffixLength = 2;
        return tradingAccountNo[..^suffixLength];
    }
}
