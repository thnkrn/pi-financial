using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Application.Models;

public record EquityAsset
{
    private const decimal Percentage = 100;

    public required string AccountNo { get; init; }
    public required string Symbol { get; init; }
    public required bool Nvdr { get; init; }
    public required decimal AverageCostPrice { get; init; }
    public required decimal AvailableVolume { get; init; }
    public required decimal SellableVolume { get; init; }
    public required decimal Amount { get; init; }
    public required bool IsNew { get; init; }
    public required decimal MarketPrice { get; init; }
    public required OrderAction Action { get; init; }
    public required OrderSide Side { get; init; }
    public required StockType StockType { get; init; }
    public decimal? RealizedPnl { get; init; }
    public IEnumerable<CorporateAction>? CorporateActions { get; init; }
    public InstrumentProfile? InstrumentProfile { get; init; }

    public decimal LendingVolume => StockType == StockType.Lending ? AvailableVolume : 0;
    public decimal MarketValue => MarketPrice * AvailableVolume;
    public decimal CostValue => StockType != StockType.Borrow ? Amount : 0;
    public decimal Upnl
    {
        get
        {
            if (IsNew || StockType == StockType.Borrow)
            {
                return 0m;
            }

            if (StockType == StockType.Short)
            {
                return CostValue - MarketValue;
            }

            return MarketValue - CostValue;
        }
    }

    public decimal UpnlPercentage
    {
        get
        {
            if (CostValue != 0 && !IsNew) return decimal.Multiply(decimal.Divide(Upnl, CostValue), Percentage);
            return 0m;
        }
    }
}

public record CreditBalanceEquityAsset : EquityAsset
{
    public required decimal Mr { get; init; }
}
