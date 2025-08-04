namespace Pi.Financial.FundService.Application.Models.Bank;

public record AtsBankAccounts(List<BankAccount> RedemptionBankAccounts, List<BankAccount> SubscriptionBankAccounts);
