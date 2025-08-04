using Microsoft.Extensions.Options;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Options;
using Pi.TfexService.Application.Services.MarketData;
using Pi.TfexService.Application.Utils;

namespace Pi.TfexService.Application.Queries.Market;

public class MarketDataQueries(IMarketDataService marketDataService, IOptions<SymbolOptions> symbolOptions) : IMarketDataQueries
{
    public async Task<MarketData?> GetMarketData(string? sid, string series, CancellationToken cancellationToken)
    {
        var listMarketData = await marketDataService.GetTicker(sid, [series]);
        var marketData = listMarketData.FirstOrDefault();

        if (marketData == null)
        {
            return null;
        }

        var symbol = GetSymbol(series);
        var instrumentCategory = GetInstrumentCategory(symbol, marketData.InstrumentCategory);
        var (tickSize, lotSize) = GetTickSize(symbolOptions, instrumentCategory, symbol);
        var (multiplier, multiplierType, multiplierUnit) = GetMultiplier(symbolOptions, instrumentCategory, symbol);

        return new MarketData(
            series,
            instrumentCategory,
            tickSize,
            lotSize,
            multiplier,
            multiplierType,
            multiplierUnit,
            marketData.Logo);
    }

    public async Task<IDictionary<string, MarketData>> GetMarketData(string? sid, List<string> listOfSeries, CancellationToken cancellationToken)
    {
        var listMarketData = await marketDataService.GetTicker(sid, listOfSeries);
        var result = new Dictionary<string, MarketData>();

        foreach (var d in listMarketData)
        {
            var symbol = GetSymbol(d.Symbol);
            var instrumentCategory = GetInstrumentCategory(symbol, d.InstrumentCategory);
            var (tickSize, lotSize) = GetTickSize(symbolOptions, instrumentCategory, symbol);
            var (multiplier, multiplierType, multiplierUnit) = GetMultiplier(symbolOptions, instrumentCategory, symbol);

            result.Add(d.Symbol, new MarketData(
                d.Symbol,
                instrumentCategory,
                tickSize,
                lotSize,
                multiplier,
                multiplierType,
                multiplierUnit,
                d.Logo));
        }

        return result;
    }

    private static string GetSymbol(string series)
    {
        // case: SET50 Index Options e.g. S50U24C700
        if (series.StartsWith("S50") && series.Length > 6)
        {
            return series[..6][..^3];
        }

        return series[..^3];
    }

    private static string GetInstrumentCategory(string symbol, string instrumentCategory)
    {
        if (string.IsNullOrWhiteSpace(instrumentCategory)
            || instrumentCategory.Equals(InstrumentCategory.Unstructure.ToString(), StringComparison.CurrentCultureIgnoreCase)
            || instrumentCategory.Equals(InstrumentCategory.Unstructured.ToString(), StringComparison.CurrentCultureIgnoreCase))
        {
            // return symbol.Equals(PreciousMetalCategory.Gd.ToString(), StringComparison.CurrentCultureIgnoreCase)
            //     ? TickSizeCategory.PreciousMetalFutures.ToString() : InstrumentCategory.Others.ToString();
            return InstrumentCategory.Others.ToString();
        }

        return instrumentCategory;
    }

    private static (decimal? tickSize, decimal? lotSize) GetTickSize(IOptions<SymbolOptions> options, string instrumentCategory, string symbol)
    {
        decimal? tickSize = null;
        decimal? lotSize = options.Value.LotSize;

        var isHaveTickSize = Enum.TryParse(instrumentCategory, true, out TickSizeCategory tickSizeCategory);
        if (isHaveTickSize)
        {
            tickSize = SeriesUtils.GetTickSize(options.Value.TickSize, tickSizeCategory, symbol);
        }

        return (tickSize, lotSize);
    }

    private static (decimal? multiplier, MultiplierType? multiplierType, MultiplierUnit? multiplierUnit) GetMultiplier(IOptions<SymbolOptions> options, string instrumentCategory, string symbol)
    {
        decimal? multiplier = null;
        MultiplierType? multiplierType = null;
        MultiplierUnit? multiplierUnit = null;

        var isHaveMultiplier = Enum.TryParse(instrumentCategory, true, out MultiplierCategory multiplierCategory);
        if (isHaveMultiplier)
        {
            multiplier = SeriesUtils.GetMultiplier(options.Value.Multiplier, multiplierCategory, symbol);
            multiplierType = multiplierCategory == MultiplierCategory.SingleStockFutures ? MultiplierType.ContractLot : MultiplierType.Multiplier;
            multiplierUnit = multiplierType == MultiplierType.ContractLot ? MultiplierUnit.Shares : MultiplierUnit.Point;
        }

        return (multiplier, multiplierType, multiplierUnit);
    }
}