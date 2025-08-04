namespace Pi.WalletService.Application.Services.GlobalEquities;

public interface IGlobalUserManagementService
{
    Task<TransferResponse> TransferMoney(string sourceAccountId, string targetAccountId, string currency, decimal amount);
}