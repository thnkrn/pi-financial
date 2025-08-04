using Pi.SetService.Application.Models;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Utils;

public static class SortHelper
{
    public static int SortAccountPositions(EquityAsset a, EquityAsset b)
    {
        // 0. Alphabet A-Z
        var symbolComparison = string.Compare(a.Symbol, b.Symbol, StringComparison.Ordinal);
        if (symbolComparison != 0)
            return symbolComparison;
        // 1. Normal
        if (a is { StockType: StockType.Normal, Nvdr: false })
        {
            if (b is { StockType: StockType.Normal, Nvdr: false })
                return b.UpnlPercentage.CompareTo(a.UpnlPercentage); // Descending by UpnlPercentage
            return -1;
        }

        if (b is { StockType: StockType.Normal, Nvdr: false })
            return 1;
        // 2. Lending
        if (a.StockType == StockType.Lending)
        {
            if (b.StockType == StockType.Lending)
                return b.UpnlPercentage.CompareTo(a.UpnlPercentage); // Descending by UpnlPercentage
            return -1;
        }

        if (b.StockType == StockType.Lending)
            return 1;
        // 3. NVDR
        if (a.Nvdr)
        {
            if (b.Nvdr)
                return b.UpnlPercentage.CompareTo(a.UpnlPercentage); // Descending by UpnlPercentage
            return -1;
        }

        if (b.Nvdr)
            return 1;
        // 4. Short
        if (a.StockType == StockType.Short)
        {
            if (b.StockType == StockType.Short)
                return b.UpnlPercentage.CompareTo(a.UpnlPercentage); // Descending by UpnlPercentage
            return -1;
        }

        if (b.StockType == StockType.Short)
            return 1;
        // 5. Borrow
        if (a.StockType == StockType.Borrow)
        {
            if (b.StockType == StockType.Borrow)
                return b.UpnlPercentage.CompareTo(a.UpnlPercentage); // Descending by UpnlPercentage
            return -1;
        }

        if (b.StockType == StockType.Borrow)
            return 1;
        // Default to descending by UpnlPercentage
        return b.UpnlPercentage.CompareTo(a.UpnlPercentage);
    }
}
