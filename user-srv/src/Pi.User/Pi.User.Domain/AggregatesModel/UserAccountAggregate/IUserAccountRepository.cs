using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.UserAccountAggregate;

public interface IUserAccountRepository : IRepository<UserAccount>
{
    Task<UserAccount> AddAsync(UserAccount userAccount);
}