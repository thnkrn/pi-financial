namespace Pi.User.API.Models;

public record UpdateBankAccountEffectiveDateRequest(Guid UserId, string CustomerCode, string BankAccountNo, string BankCode, string? BankBranchCode, DateTime EffectiveDate, DateTime EndDate);
