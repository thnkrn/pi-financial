using Pi.Common.Features;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Domain.Exceptions;
using Pi.TfexService.Domain.Models.InitialMargin;

namespace Pi.TfexService.Application.Queries.Margin;

public class InitialMarginQueries(
    IInitialMarginRepository initialMarginRepository,
    ISetTradeService setTradeService,
    IFeatureService featureService) : IInitialMarginQueries
{
    private static readonly HashSet<char> ValidMonthCodes = ['F', 'G', 'H', 'J', 'K', 'M', 'N', 'Q', 'U', 'V', 'X', 'Z'];

    public async Task<InitialMarginDto> GetInitialMargin(string series, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(series) || series.Length < 3)
            throw new ArgumentException("Invalid Series");

        // Stock Futures: {symbol}{expire-month}{expire-year}
        // For example: GCZ17 (GC December 2017)
        // More info at: https://bettertrader.co/online-trading-academy/futures-symbols-and-months.html

        // validate month code
        if (!ValidMonthCodes.Contains(series[^3..][0]))
        {
            throw new ArgumentException("Invalid Expiration month");
        }

        var data = await initialMarginRepository.GetInitialMargin(series[..^3], cancellationToken);
        if (data == null)
        {
            throw new SetTradeNotFoundException("Series not found");
        }

        return new InitialMarginDto(
            data.Symbol,
            data.ProductType,
            data.Im,
            data.ImOutright,
            data.ImSpread
        );
    }

    public async Task<decimal> GetEstRequiredInitialMargin(string accountCode, Side side, string series, int placingUnit, CancellationToken cancellationToken)
    {
        if (!IsFutures(series))
        {
            throw new ArgumentException("Invalid Series");
        }

        // prepare data for calculation
        var margin = await GetInitialMargin(series, cancellationToken);
        var portfolio = await setTradeService.GetPortfolio(accountCode, cancellationToken);
        var groupedPositions = portfolio.PortfolioList.GroupBy(x => x.Underlying)
            .Select(p => new
            {
                Underlying = p.Key,
                TotalLong = p.Where(x => x.HasLongPosition).Sum(x => x.ActualLongPosition),
                TotalShort = p.Where(x => x.HasShortPosition).Sum(x => x.ActualShortPosition),
                Symbols = p.Select(x => new
                {
                    x.Symbol,
                    Side = x.HasLongPosition ? Side.Long : Side.Short,
                    ActualPosition = x.HasLongPosition ? x.ActualLongPosition : x.ActualShortPosition
                }).ToDictionary(x => x.Symbol, x => new { x.Symbol, x.Side, x.ActualPosition })
            }).ToDictionary(x => x.Underlying, x => new
            {
                x.TotalLong,
                x.TotalShort,
                x.Symbols
            });

        // finding main side, main series & available unit
        var underlying = series[..^3];
        var currentPositions = groupedPositions.GetValueOrDefault(underlying);
        var mainSide = currentPositions?.TotalLong >= currentPositions?.TotalShort ? Side.Long : Side.Short;
        var mainSeries = currentPositions?.Symbols.Values
            .Where(x => x.Side == mainSide)
            .OrderByDescending(x => x.ActualPosition)
            .First().Symbol;
        var availableUnit = Math.Abs((currentPositions?.TotalLong ?? 0) - (currentPositions?.TotalShort ?? 0));

        // scenario 1: no current positions or same side
        if (currentPositions == null || mainSide == side || featureService.IsOn(Features.DefaultTfexInitialMargin))
        {
            return margin.ImOutright * placingUnit;
        }

        // scenario 2: same series & different side
        if (mainSeries == series)
        {
            return (Math.Min(placingUnit, availableUnit) * -1 * margin.ImOutright)
                   + (Math.Max(placingUnit - availableUnit, 0) * margin.ImOutright);
        }

        // scenario 3: different series & different side
        var matchedPosition = (margin.ImSpread - margin.ImOutright) * Math.Min(placingUnit, availableUnit);
        var unmatchedPosition = margin.ImOutright * Math.Max(placingUnit - availableUnit, 0);
        return matchedPosition + unmatchedPosition;
    }

    private bool IsFutures(string input)
    {
        if (input.Length > 3)
        {
            var monthCode = input[^3];
            var yearCode = input[^2..];

            if (ValidMonthCodes.Contains(monthCode) && int.TryParse(yearCode, out _))
            {
                return true;
            }
        }

        return false;
    }
}