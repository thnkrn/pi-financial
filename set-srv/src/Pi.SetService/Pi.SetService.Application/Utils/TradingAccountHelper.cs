namespace Pi.SetService.Application.Utils;

public static class TradingAccountHelper
{
    public static string? GetCustCodeBySetTradingAccountNo(string tradingAccountNo)
    {
        if (!tradingAccountNo.EndsWith("-1") && !tradingAccountNo.EndsWith("-6") && !tradingAccountNo.EndsWith("-8")) return null;

        const int suffixLength = 2;
        return tradingAccountNo[..^suffixLength];
    }

    public static string GetCustCodeBySetAccountNo(string accountNo)
    {
        const int suffixLength = 1;
        return accountNo[..^suffixLength];
    }
}
