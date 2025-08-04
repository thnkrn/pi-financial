using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.API.Models;

public record SetAccountSummaryResponse
{
    public required string TradingAccountNo { get; init; }
    public required TradingAccountType TradingAccountType { get; init; }
    public required string CustomerCode { get; init; }
    public required string AccountCode { get; init; }
    public required string AsOfDate { get; init; }
    public required decimal TotalCost { get; init; }
    public required decimal TotalUpnl { get; init; }
    public required decimal TotalUpnlPercentage { get; init; }
    public required decimal CashBalance { get; init; }
    public required bool SblEnabled { get; set; }
    public required decimal TotalValue { get; init; }
    public required decimal TotalMarketValue { get; init; }
    public required List<SetAccountAssetsResponse> Assets { get; init; } = new();
}

public record SetCashAccountSummaryResponse : SetAccountSummaryResponse
{
    public required decimal LineAvailable { get; init; }
}

public record SetCreditBalanceAccountSummaryResponse : SetAccountSummaryResponse
{
    public required decimal AccountValue { get; init; }
    public required decimal LongMarketValue { get; init; }
    public required decimal ShortMarketValue { get; init; }
    public required decimal MarginLoan { get; init; }
    public required decimal LongCostValue { get; init; }
    public required decimal ShortCostValue { get; init; }
    public required decimal ExcessEquity { get; init; }
    public required decimal Liabilities { get; init; }
}
