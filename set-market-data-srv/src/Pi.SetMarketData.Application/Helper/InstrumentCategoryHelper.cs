using Pi.SetMarketData.Application.Constants;

namespace Pi.SetMarketData.Application.Helper;

public static class InstrumentCategoryHelper
{
    public static string MapMarketSegment(string marketSegmentValue)
    {
        return marketSegmentValue switch
        {
            InstrumentConstants.SET => InstrumentConstants.SET,
            InstrumentConstants.TXE => InstrumentConstants.TXE,
            InstrumentConstants.MAI => InstrumentConstants.MAI,
            InstrumentConstants.TXI => InstrumentConstants.TXI,
            InstrumentConstants.TXS => InstrumentConstants.TXS,
            InstrumentConstants.TXM => InstrumentConstants.TXM,
            InstrumentConstants.TXC => InstrumentConstants.TXC,
            _ => string.Empty
        };
    }

    public static int MapMarketCode(int marketCodeValue)
    {
        var marketCode = 0;
        switch (marketCodeValue)
        {
            case 11:
                marketCode = 11;
                return marketCode;
            case 13:
                marketCode = 13;
                return marketCode;
            case 101:
                marketCode = 101;
                return marketCode;
            case 102:
                marketCode = 102;
                return marketCode;
            case 104:
                marketCode = 104;
                return marketCode;
            case 105:
                marketCode = 105;
                return marketCode;
            default:
                return marketCode;
        }
    }


    public static string MapInstrumentCategory(string financialProduct, string marketSegment, int marketCode,
        string underlyingName)
    {
        return financialProduct switch
        {
            InstrumentConstants.CS or InstrumentConstants.PS => MapStockCategory(marketSegment, marketCode),
            InstrumentConstants.ETF => MapEtfCategory(marketSegment, marketCode),
            InstrumentConstants.W => MapWarrantCategory(marketSegment, marketCode),
            InstrumentConstants.DWC or InstrumentConstants.DWP => MapDerivativesWarrantCategory(marketSegment,
                marketCode),
            InstrumentConstants.DR => MapDepositaryReceiptCategory(marketSegment, marketCode),
            InstrumentConstants.FC => MapFuturesCategory(marketSegment, marketCode, underlyingName),
            InstrumentConstants.OEC or InstrumentConstants.OEP => MapOptionsCategory(marketSegment, marketCode),
            InstrumentConstants.IDX => InstrumentConstants.SETIndices,
            InstrumentConstants.TSR or InstrumentConstants.CMB or InstrumentConstants.UL or
                InstrumentConstants.SPT or InstrumentConstants.WEC or InstrumentConstants.WEP =>
                MapIgnoredCategory(marketSegment),
            _ => InstrumentConstants.Unstructured
        };
    }

    private static string MapStockCategory(string marketSegment, int marketCode)
    {
        return marketSegment switch
        {
            InstrumentConstants.SET
            or InstrumentConstants.MAI when marketCode == 11
                => InstrumentConstants.ThaiStocks,
            _ => InstrumentConstants.Unstructured
        };
    }

    private static string MapEtfCategory(string marketSegment, int marketCode)
    {
        return marketSegment == InstrumentConstants.SET && marketCode == 11
            ? InstrumentConstants.ThaiETFs
            : InstrumentConstants.Unstructured;
    }

    private static string MapWarrantCategory(string marketSegment, int marketCode)
    {
        return marketSegment == InstrumentConstants.SET && marketCode == 11
            ? InstrumentConstants.ThaiStockWarrants
            : InstrumentConstants.Unstructured;
    }

    private static string MapDerivativesWarrantCategory(string marketSegment, int marketCode)
    {
        if (marketSegment == InstrumentConstants.SET)
            return marketCode switch
            {
                11 => InstrumentConstants.ThaiDerivativeWarrants,
                13 => InstrumentConstants.ForeignDerivativeWarrants,
                _ => InstrumentConstants.Unstructured
            };
        return InstrumentConstants.Unstructured;
    }

    private static string MapDepositaryReceiptCategory(string marketSegment, int marketCode)
    {
        return marketSegment == InstrumentConstants.SET && marketCode == 13
            ? InstrumentConstants.DRs
            : InstrumentConstants.Unstructured;
    }

    private static string MapFuturesCategory(string marketSegment, int marketCode, string underlyingName)
    {
        return (marketSegment, marketCode) switch
        {
            (InstrumentConstants.TXI, 101) => MapTxiFutures(underlyingName),
            (InstrumentConstants.TXS, 102) => InstrumentConstants.SingleStockFutures,
            (InstrumentConstants.TXM, 104) => InstrumentConstants.PreciousMetalFutures,
            (InstrumentConstants.TXC, 105) => InstrumentConstants.CurrencyFutures,
            (InstrumentConstants.SET or InstrumentConstants.TXE or InstrumentConstants.MAI, _) => InstrumentConstants
                .Ignored,
            _ => InstrumentConstants.Unstructured
        };
    }

    private static string MapTxiFutures(string underlyingName)
    {
        return underlyingName == InstrumentConstants.SET50
            ? InstrumentConstants.SET50IndexFutures
            : InstrumentConstants.SectorIndexFutures;
    }

    private static string MapOptionsCategory(string marketSegment, int marketCode)
    {
        return marketSegment == InstrumentConstants.TXI && marketCode == 101
            ? InstrumentConstants.SET50IndexOptions
            : InstrumentConstants.Unstructured;
    }

    private static string MapIgnoredCategory(string marketSegment)
    {
        return marketSegment == InstrumentConstants.SET ||
               marketSegment == InstrumentConstants.TXE ||
               marketSegment == InstrumentConstants.MAI
            ? InstrumentConstants.Ignored
            : InstrumentConstants.Unstructured;
    }
}