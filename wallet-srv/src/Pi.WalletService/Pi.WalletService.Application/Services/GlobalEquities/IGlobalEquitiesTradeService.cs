namespace Pi.WalletService.Application.Services.GlobalEquities;


public interface IGlobalTradeService
{
    Task<AccountSummaryResponse> GetAccountSummary(string accountId, string currency);
    Task<decimal> GetAvailableWithdrawalAmount(string accountId, string currency);
}