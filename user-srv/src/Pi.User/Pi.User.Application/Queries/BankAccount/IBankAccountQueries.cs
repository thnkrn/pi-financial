using Pi.User.Application.Models.BankAccount;

namespace Pi.User.Application.Queries.BankAccount;

public interface IBankAccountQueries
{
    Task<BankAccountDto> GetBankAccountByUserId(Guid userId);
}