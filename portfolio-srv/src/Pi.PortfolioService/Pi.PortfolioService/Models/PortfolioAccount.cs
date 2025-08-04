namespace Pi.PortfolioService.Models;

public record PortfolioAccount(
    string AccountType,
    string AccountId,
    string AccountNoForDisplay,
    string TradingAccountNo,
    string CustCode,
    bool SblFlag,
    decimal TotalMarketValue,
    decimal CashBalance,
    decimal Upnl,
    string ErrorMessage
)
{
    public decimal TotalValue { get; init; } = TotalMarketValue + CashBalance;
    public decimal TotalCostValue => TotalMarketValue - Upnl;
    public decimal UpnlPercentage =>
        TotalCostValue != 0 ? (TotalMarketValue / TotalCostValue - 1) * 100 : 0;
};
