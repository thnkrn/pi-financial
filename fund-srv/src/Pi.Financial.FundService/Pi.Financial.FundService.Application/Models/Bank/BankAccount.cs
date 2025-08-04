namespace Pi.Financial.FundService.Application.Models.Bank;

public record BankAccount(string BankCode, string BankAccountNo, string BankBranchCode, bool IsDefault = false, string? FinnetCustomerNo = null);
