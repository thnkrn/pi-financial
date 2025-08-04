using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.BankAccountAggregate;

namespace Pi.User.Infrastructure.Repositories;

public class BankAccountRepository : IBankAccountRepository
{
    private readonly UserDbContext _userDbContext;
    private readonly ILogger<BankAccountRepository> _logger;
    public IUnitOfWork UnitOfWork => _userDbContext;

    public BankAccountRepository(
        UserDbContext userDbContext,
        ILogger<BankAccountRepository> logger)
    {
        _userDbContext = userDbContext;
        _logger = logger;
    }

    public async Task<BankAccount> AddAsync(BankAccount bankAccount)
    {
        return (await _userDbContext.BankAccounts.AddAsync(bankAccount)).Entity;
    }

    public async Task<BankAccount?> GetByUserIdAsync(Guid userId)
    {
        var request = await _userDbContext.BankAccounts.FirstOrDefaultAsync(n => n.UserId.Equals(userId));
        return request;
    }

    public void Delete(BankAccount bankAccount)
    {
        _userDbContext.BankAccounts.Remove(bankAccount);
    }
}