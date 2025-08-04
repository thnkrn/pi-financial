namespace Pi.PortfolioService.DomainModels;

public record PortfolioSummary(
    DateTimeOffset AsOfDate,
    string Currency,
    decimal Liabilities,
    List<PortfolioAccount> PortfolioAccounts,
    List<PortfolioWalletCategorized> PortfolioWalletCategorizeds,
    List<PortfolioErrorAccount> PortfolioErrorAccounts,
    List<GeneralError> GeneralErrors)
{
    public decimal TotalValue => PortfolioAccounts.Sum(pa => pa.TotalValue);
    public decimal TotalMarketValue => PortfolioAccounts.Sum(pa => pa.TotalMarketValue);
    public decimal TotalCostValue => PortfolioAccounts.Sum(pa => pa.TotalCostValue);
    public decimal CashBalance => PortfolioAccounts.Sum(pa => pa.CashBalance);
    public decimal Upnl => PortfolioAccounts.Sum(pa => pa.Upnl);
    public decimal UpnlPercentage =>
        TotalCostValue != 0 ? (TotalMarketValue / TotalCostValue - 1) * 100 : 0;

    public static PortfolioSummary Default(string? currency = null)
    {
        return new PortfolioSummary(default, currency ?? "USD", 0, new(), new(), new(), new());
    }
};
