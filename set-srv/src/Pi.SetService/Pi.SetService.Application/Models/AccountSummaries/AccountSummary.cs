using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Models.AccountSummaries;

public abstract record AccountSummary : ISblSummary
{
    public required string TradingAccountNo { get; init; }
    public required string CustomerCode { get; init; }
    public required string AccountNo { get; init; }
    public required DateTime AsOfDate { get; init; }
    public required string TraderId { get; init; }
    public required decimal CreditLimit { get; init; }
    public required decimal BuyCredit { get; init; }
    public required decimal CashBalance { get; init; }
    public required TradingAccountType AccountType { get; init; }
    public required IEnumerable<EquityAsset> Assets { get; init; }
    public bool SblEnabled { get; init; }
    public abstract decimal TotalValue { get; }

    public decimal TotalMarketValue
    {
        get
        {
            if (SblEnabled) return LongMarketValue - ShortMarketValue;

            return Assets.Any()
                ? Assets.Sum(q => !q.IsNew && q.StockType != StockType.Borrow ? q.MarketValue : decimal.Zero)
                : decimal.Zero;
        }
    }

    public decimal TotalCost
    {
        get
        {
            if (SblEnabled) return LongCostValue - ShortCostValue;

            return Assets.Any()
                ? Assets.Sum(q => !q.IsNew && q.StockType != StockType.Borrow ? q.CostValue : decimal.Zero)
                : decimal.Zero;
        }
    }

    public decimal TotalUpnl
    {
        get
        {
            if (SblEnabled) return TotalMarketValue - TotalCost;

            return Assets.Any() ? Assets.Sum(q => !q.IsNew ? q.Upnl : decimal.Zero) : decimal.Zero;
        }
    }

    public decimal TotalUpnlPercentage => TotalCost == decimal.Zero ? decimal.Zero : TotalUpnl * 100 / TotalCost;

    public virtual decimal LongMarketValue
    {
        get
        {
            return SblEnabled
                ? Assets.Sum(q => !q.IsNew && q.StockType != StockType.Short && q.StockType != StockType.Borrow ? q.MarketValue : decimal.Zero)
                : decimal.Zero;
        }
    }

    public virtual decimal ShortMarketValue
    {
        get
        {
            return SblEnabled
                ? Assets.Sum(q => q is { IsNew: false, StockType: StockType.Short } ? q.MarketValue : decimal.Zero)
                : decimal.Zero;
        }
    }

    public decimal LongCostValue
    {
        get
        {
            return SblEnabled
                ? Assets.Sum(q => !q.IsNew && q.StockType != StockType.Short && q.StockType != StockType.Borrow ? q.CostValue : decimal.Zero)
                : decimal.Zero;
        }
    }

    public decimal ShortCostValue
    {
        get
        {
            return SblEnabled
                ? Assets.Sum(q => q is { IsNew: false, StockType: StockType.Short } ? q.CostValue : decimal.Zero)
                : decimal.Zero;
        }
    }
}
