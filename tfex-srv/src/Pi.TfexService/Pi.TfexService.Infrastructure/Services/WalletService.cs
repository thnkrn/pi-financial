using Microsoft.Extensions.Logging;
using Pi.Client.WalletService.Api;
using Pi.Client.WalletService.Model;
using Pi.TfexService.Application.Services.Wallet;

namespace Pi.TfexService.Infrastructure.Services;

public class WalletService(IWalletApiAsync walletApiAsync, ILogger<WalletService> logger) : IWalletService
{
    public async Task<decimal> GetWalletBalance(string userId, string accountCode, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(accountCode) || accountCode.Length < 8)
            {
                throw new ArgumentException("Invalid account code", nameof(accountCode));
            }

            var custCode = accountCode[..7];
            var walletBalance = await walletApiAsync.InternalWalletProductWithdrawBalanceGetAsync(
                userId,
                PiWalletServiceIntegrationEventsAggregatesModelProduct.Derivatives,
                custCode,
                cancellationToken);

            return walletBalance.Data.Amount;
        }
        catch (Exception e)
        {
            logger.LogError("Failed to get wallet balance for userId: {User}, accountCode: {Account}. Exception: {Exception}", userId, accountCode, e);
            throw;
        }
    }
}