using Pi.Client.OnboardService.Model;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Enums;

namespace Pi.Financial.FundService.Application.Services.OnboardService
{
    public interface IOnboardService
    {
        Task UpdateOpenFundAccountStatus(Guid openAccountRequestId, OpenFundAccountStatus status);
        Task<Crs> GetUserCrsByUserId(Guid userId);
        Task<TradingAccountInfo?> GetMutualFundTradingAccountByCustCodeAsync(string custCode, CancellationToken cancellationToken = default);
        Task<BankAccount?> GetATSBankAccountForDepositByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default);
        Task<BankAccount?> GetATSBankAccountForWithdrawalByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default);
        Task<AtsBankAccounts> GetATSBankAccountsByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default);
        Task<DateTime?> GetDopaSuccessInfoByUserId(Guid userId);

        Task<List<PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto>> GetDocumentByUserId(
            Guid userId, CancellationToken cancellationToken = default);
    }
}
