namespace Pi.PortfolioService.API.Models;

public record PortfolioSummaryResponse(
    long AsOfDate,
    string Currency,
    string TotalValue,
    string TotalMarketValue,
    string TotalCostValue,
    string CashBalance,
    string Liabilities,
    string Upnl,
    string UpnlPercentage,
    IEnumerable<PortfolioAccountResponse> AccountList,
    IEnumerable<PortfolioWalletCategorizedResponse> WalletCategorized,
    IEnumerable<PortfolioErrorAccountResponse> ErrorAccountList,
    IEnumerable<GeneralError> GeneralErrors);

public record PortfolioAccountResponse(
    string AccountType,
    string AccountId,
    string AccountNoForDisplay,
    string TradingAccountNo,
    string CustCode,
    bool SblFlag,
    string TotalValue,
    string TotalMarketValue,
    string TotalCostValue,
    string CashBalance,
    string Upnl,
    string UpnlPercentage,
    string ErrorMessage
);

public record PortfolioErrorAccountResponse(
    string AccountType,
    string AccountId,
    string ErrorMessage,
    string AccountNoForDisplay
);

public record PortfolioWalletCategorizedResponse(
    string AccountType,
    string TotalValue,
    string TotalMarketValue,
    string TotalCostValue,
    string CashBalance,
    string Upnl,
    string UpnlPercentage,
    string AssetRatioInAllAsset
);

public record GeneralError(
    string AccountType,
    string Error
);
