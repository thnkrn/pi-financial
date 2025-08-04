using Pi.Financial.FundService.Application.Models.Bank;

namespace Pi.Financial.FundService.Application.Models.Customer;

public record UpdateCustomerJuristicAccount(
    string JuristicNumber,
    string AccountId,
    string IcLicense,
    string AccountOpenDate,
    string MailingAddressSameAsFlag,
    string MailingMethod,
    string InvestmentObjective,
    string InvestmentObjectiveOther,
    List<BankAccount> RedemptionBankAccounts,
    List<BankAccount> SubscriptionBankAccounts,
    bool Approved
);
