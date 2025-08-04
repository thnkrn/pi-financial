using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.BankAccountAggregate;

public interface IBankAccountRepository : IRepository<BankAccount>
{
    Task<BankAccount> AddAsync(BankAccount bankAccount);
    Task<BankAccount?> GetByUserIdAsync(Guid userId);
    void Delete(BankAccount bankAccount);
}