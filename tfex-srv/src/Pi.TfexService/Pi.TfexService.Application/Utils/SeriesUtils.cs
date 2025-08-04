using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Options;

namespace Pi.TfexService.Application.Utils;

public static class SeriesUtils
{
    public static decimal? GetMultiplier(Multiplier multiplier, MultiplierCategory multiplierCategory, string symbol)
    {
        return multiplierCategory switch
        {
            MultiplierCategory.Set50IndexFutures => multiplier.Set50IndexFutures,
            MultiplierCategory.Set50IndexOptions => multiplier.Set50IndexOptions,
            MultiplierCategory.SingleStockFutures => multiplier.SingleStockFutures,
            MultiplierCategory.SectorIndexFutures => GetSectorIndexFutures(multiplier.SectorIndexFutures, symbol),
            _ => null
        };
    }

    public static decimal? GetTickSize(TickSize tickSize, TickSizeCategory tickSizeCategory, string symbol)
    {
        return tickSizeCategory switch
        {
            TickSizeCategory.Set50IndexFutures => tickSize.Set50IndexFutures,
            TickSizeCategory.Set50IndexOptions => tickSize.Set50IndexOptions,
            TickSizeCategory.SingleStockFutures => tickSize.SingleStockFutures,
            TickSizeCategory.SectorIndexFutures => GetSectorIndexFutures(tickSize.SectorIndexFutures, symbol),
            TickSizeCategory.PreciousMetalFutures => GetPreciousMetalFutures(tickSize.PreciousMetalFutures, symbol),
            TickSizeCategory.CurrencyFutures => GetCurrencyFutures(tickSize.CurrencyFutures, symbol),
            TickSizeCategory.Others => GetOtherFutures(tickSize.OtherFutures, symbol),
            _ => null
        };
    }

    private static decimal? GetSectorIndexFutures(SectorIndexFutures sectorIndexFutures, string symbol)
    {
        var isValid = Enum.TryParse(symbol, true, out SectorCategory sectorCategory);
        if (!isValid)
        {
            return null;
        }

        return sectorCategory switch
        {
            SectorCategory.Bank => sectorIndexFutures.Bank,
            SectorCategory.Ict => sectorIndexFutures.Ict,
            SectorCategory.Energ => sectorIndexFutures.Energ,
            SectorCategory.Comm => sectorIndexFutures.Comm,
            SectorCategory.Food => sectorIndexFutures.Food,
            _ => null
        };
    }

    private static decimal? GetPreciousMetalFutures(PreciousMetalFutures preciousMetalFutures, string symbol)
    {
        var isValid = Enum.TryParse(symbol, true, out PreciousMetalCategory preciousMetalCategory);
        if (!isValid)
        {
            return null;
        }

        return preciousMetalCategory switch
        {
            PreciousMetalCategory.Gf => preciousMetalFutures.GoldFutures,
            PreciousMetalCategory.Gf10 => preciousMetalFutures.GoldFutures10,
            PreciousMetalCategory.Go => preciousMetalFutures.GoldOnlineFutures,
            PreciousMetalCategory.Svf => preciousMetalFutures.SilverOnlineFutures,
            PreciousMetalCategory.Gd => preciousMetalFutures.GoldD,
            _ => null
        };
    }

    private static decimal? GetCurrencyFutures(CurrencyFutures currencyFutures, string symbol)
    {
        var isValid = Enum.TryParse(symbol, true, out CurrencyCategory currencyCategory);
        if (!isValid)
        {
            return null;
        }

        return currencyCategory switch
        {
            CurrencyCategory.Usd => currencyFutures.UsdFutures,
            CurrencyCategory.Eur => currencyFutures.EurFutures,
            CurrencyCategory.Jpy => currencyFutures.JpyFutures,
            CurrencyCategory.EurUsd => currencyFutures.EurUsdFutures,
            CurrencyCategory.UsdJpy => currencyFutures.UsdJpyFutures,
            _ => null
        };
    }

    private static decimal? GetOtherFutures(OtherFutures otherFutures, string symbol)
    {
        var isValid = Enum.TryParse(symbol, true, out OtherCategory otherCategory);
        if (!isValid)
        {
            return null;
        }

        return otherCategory switch
        {
            OtherCategory.Rss3 => otherFutures.Rss3Futures,
            OtherCategory.Rss3d => otherFutures.Rss3dFutures,
            OtherCategory.Jrf => otherFutures.JrfFutures,
            _ => null
        };
    }
}