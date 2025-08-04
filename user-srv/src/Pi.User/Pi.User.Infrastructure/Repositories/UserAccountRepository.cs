using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.UserAccountAggregate;

namespace Pi.User.Infrastructure.Repositories;

public class UserAccountRepository(UserDbContext userDbContext) : IUserAccountRepository
{
    public IUnitOfWork UnitOfWork => userDbContext;

    public async Task<UserAccount> AddAsync(UserAccount userAccount)
    {
        return (await userDbContext.UserAccounts.AddAsync(userAccount)).Entity;
    }
}