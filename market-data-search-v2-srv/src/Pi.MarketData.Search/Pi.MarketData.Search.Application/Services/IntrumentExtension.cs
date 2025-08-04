using Pi.MarketData.Search.Domain.Models;

namespace Pi.MarketData.Search.Application.Services;

public static class IntrumentExtension
{
    public static InstrumentCategory GetInstrumentCategory(string? category)
    {
        return category switch
        {
            "Thai Stocks" => InstrumentCategory.ThaiStocks,
            "GlobalStocks" => InstrumentCategory.GlobalStocks,
            "Funds" => InstrumentCategory.Funds,
            "GlobalETFs" => InstrumentCategory.GlobalETFs,
            "Thai ETFs" => InstrumentCategory.ThaiETFs,
            "SET50 Index Futures" => InstrumentCategory.SET50IndexFutures,
            "Single Stock Futures" => InstrumentCategory.SingleStockFutures,
            "Currency Futures" => InstrumentCategory.CurrencyFutures,
            "Precious Metal Futures" => InstrumentCategory.PreciousMetalFutures,
            "Sector Index Futures" => InstrumentCategory.SectorIndexFutures,
            "SET50 Index Options" => InstrumentCategory.SET50IndexOptions,
            "Thai Stock Warrants" => InstrumentCategory.ThaiStockWarrants,
            "Thai Derivative Warrants" => InstrumentCategory.ThaiDerivativeWarrants,
            "Foreign Derivative Warrants" => InstrumentCategory.ForeignDerivativeWarrants,
            "DRs" => InstrumentCategory.DRs,
            "SET Indices" => InstrumentCategory.SETIndices,
            "Global Indices" => InstrumentCategory.GlobalIndices,
            "Currencies" => InstrumentCategory.Currencies,
            "Commodities" => InstrumentCategory.Commodities,
            _ => InstrumentCategory.Unstructure
        };
    }

    public static InstrumentType GetInstrumentType(string? type)
    {
        return Enum.TryParse(type, out InstrumentType result) ? result : InstrumentType.Unknown;
    }
}
