using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Models.AccountSummaries;

public record CreditBalanceAccountSummary : AccountSummary, ICreditBalanceSummary
{
    public override decimal TotalValue => EquityValue;

    public required decimal Mr { get; init; }
    public required decimal EquityValue { get; init; }
    public required decimal ExcessEquity { get; init; }
    public required decimal Liabilities { get; init; }
    public required decimal Lmv { get; init; }
    public required decimal Smv { get; init; }

    public override decimal LongMarketValue => Lmv != decimal.Zero ? Lmv : base.LongMarketValue;
    public override decimal ShortMarketValue => Smv != decimal.Zero ? Smv : base.ShortMarketValue;

    public decimal MarginLoan
    {
        get
        {
            return Mr - Assets.Sum(q =>
                q is { IsNew: false, StockType: StockType.Short }
                    ? (q as CreditBalanceEquityAsset)?.Mr ?? decimal.Zero
                    : decimal.Zero);
        }
    }
}
