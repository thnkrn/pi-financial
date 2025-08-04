using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Domain.Utils;

public static class SetHelper
{
    private const string NvdrSuffix = "-R";

    public static string CleanSymbol(string symbol, Ttf ttf)
    {
        return IsNvdr(ttf) && symbol.EndsWith(NvdrSuffix) ? symbol[..^NvdrSuffix.Length] : symbol;
    }

    public static bool IsNvdr(Ttf ttf)
    {
        return ttf == Ttf.Nvdr;
    }
}
