using Pi.WalletService.Application.Models;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Services.OnboardService;

public record CustomerInfo(
    string CustomerCode,
    string FirstnameTh,
    string LastnameTh,
    string FirstnameEn,
    string LastnameEn,
    string TaxId);

public interface IOnboardService
{
    public Task<BankAccount?> GetBankAccount(string customerCode, Product product, TransactionType transactionType);
    public Task<CustomerInfo> GetCustomerInfo(string customerCode);
    public Task<string?> GetMarketingId(string customerCode, string tradingAccount);
}
