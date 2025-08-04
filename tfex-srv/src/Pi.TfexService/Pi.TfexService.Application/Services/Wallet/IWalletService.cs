namespace Pi.TfexService.Application.Services.Wallet;

public interface IWalletService
{
    Task<decimal> GetWalletBalance(string userId, string accountCode, CancellationToken cancellationToken);
}