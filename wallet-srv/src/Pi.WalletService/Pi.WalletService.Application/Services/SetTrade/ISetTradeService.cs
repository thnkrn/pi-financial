namespace Pi.WalletService.Application.Services.SetTrade
{
    public interface ISetTradeService
    {
        Task<decimal> GetWithdrawalBalance(string accountNo);
        Task<SetTradeDepositWithdrawResponse> CashDeposit(string userId, string transactionId,
            string accountNo, decimal amount);

        Task<SetTradeDepositWithdrawResponse> CashWithdraw(string userId, string transactionId,
            string accountNo, decimal amount);

        Task<string> GetAccessToken();
        Task<string> GenerateAccessToken();
    }
}
