using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.BankAccountAggregate;

public interface IBankAccountV2Repository : IRepository<BankAccountV2>
{
    Task AddAsync(BankAccountV2 bankAccountV2);
}