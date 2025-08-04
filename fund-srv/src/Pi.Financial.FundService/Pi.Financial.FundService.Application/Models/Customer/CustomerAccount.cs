using Pi.Financial.FundService.Application.Models.Bank;

namespace Pi.Financial.FundService.Application.Models.Customer;

public record CustomerAccount(
    string AccountId,
    string SaleLicense,
    List<BankAccount> RedemptionBankAccount,
    List<BankAccount> SubscriptionBankAccount,
    DateOnly OpenDate);
