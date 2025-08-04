namespace Pi.PortfolioService.Models;

public record PortfolioWalletCategorized(
    string AccountType,
    decimal TotalMarketValue,
    decimal CashBalance,
    decimal Upnl,
    decimal AssetRatioInAllAsset
)
{
    public decimal TotalValue { get; init; } = TotalMarketValue + CashBalance;
    public decimal TotalCostValue => TotalMarketValue - Upnl;
    public decimal UpnlPercentage => TotalCostValue != 0 ? (TotalMarketValue / TotalCostValue - 1) * 100 : 0;
};
