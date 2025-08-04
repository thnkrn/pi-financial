using Microsoft.EntityFrameworkCore;
using Pi.Common.SeedWork;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;

namespace Pi.WalletService.Infrastructure.Repositories;

public class TransactionHistoryRepository : ITransactionHistoryRepository
{
    private readonly WalletDbContext _walletDbContext;

    public IUnitOfWork UnitOfWork => _walletDbContext;

    public TransactionHistoryRepository(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;

    }

    public TransactionHistory Create(TransactionHistory transactionHistory)
    {
        return _walletDbContext.TransactionHistories.Add(transactionHistory).Entity;
    }
}