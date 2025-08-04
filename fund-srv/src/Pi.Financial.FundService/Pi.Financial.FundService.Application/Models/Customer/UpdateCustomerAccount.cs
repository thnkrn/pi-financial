using Pi.Financial.FundService.Application.Models.Bank;

namespace Pi.Financial.FundService.Application.Models.Customer;

public record UpdateCustomerAccount(
    string IdentificationCardType,
    string PassportCountry,
    string CardNumber,
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
