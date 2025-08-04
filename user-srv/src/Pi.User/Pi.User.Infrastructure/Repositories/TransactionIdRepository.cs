using Microsoft.EntityFrameworkCore;
using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.TransactionIdAggregate;

namespace Pi.User.Infrastructure.Repositories;

public class TransactionIdRepository : ITransactionIdRepository
{
    private readonly UserDbContext _userDbContext;
    public IUnitOfWork UnitOfWork => _userDbContext;

    public TransactionIdRepository(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public async Task<TransactionId?> GetTransactionAsync(string referId, CancellationToken cancellationToken = default)
    {
        var transactionId = await this._userDbContext.TransactionIds.Where(x => x.ReferId == referId).SingleOrDefaultAsync(cancellationToken: cancellationToken);
        return transactionId;
    }

    public async Task<TransactionId> GetNextAsync(string prefix, DateOnly date, string referId, string customerCode, CancellationToken cancellationToken = default)
    {
        var transactionId = await this._userDbContext.TransactionIds.FindAsync(prefix, date, cancellationToken);
        if (transactionId == null)
        {
            transactionId = new TransactionId(prefix, date, referId, customerCode, 1);
            await this._userDbContext.AddAsync(transactionId, cancellationToken);
        }

        return transactionId.Next();
    }
}