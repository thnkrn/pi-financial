using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Infrastructure.Extensions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using CashDepositState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashDepositState;

namespace Pi.WalletService.Infrastructure.Repositories;

public class CashDepositRepository : ICashDepositRepository
{
    private readonly WalletDbContext _walletDbContext;

    public CashDepositRepository(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<CashDepositState?> Get(string transactionNo)
    {
        var transaction = await _walletDbContext
            .Set<CashDepositState>()
            .SingleOrDefaultAsync(t => t.TransactionNo == transactionNo);

        return transaction;
    }

    public async Task<CashDepositState?> Get(string userId, Product product, string transactionNo)
    {
        var transaction = await _walletDbContext
            .Set<CashDepositState>()
            .SingleOrDefaultAsync(t => t.TransactionNo == transactionNo && t.UserId == userId && t.Product == product);

        return transaction;
    }

    public async Task<List<CashDepositState>> Get(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<CashDepositState>? filters)
    {
        var query = _walletDbContext
            .Set<CashDepositState>()
            .AsQueryable()
            .WhereByFilters(filters);

        if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(orderDir))
        {
            query = query.OrderByProperty(orderBy, orderDir);
        }
        else
        {
            query = query.OrderByDescending(q => q.CreatedAt);
        }

        return await query.Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountTransactions(IQueryFilter<CashDepositState>? filters)
    {
        return await _walletDbContext
            .Set<CashDepositState>()
            .AsQueryable()
            .WhereByFilters(filters)
            .CountAsync();
    }

    public async Task<bool> UpdateAccountCodeByTransactionNoAndState(string transactionNo, string state, string accountCode)
    {
        var transaction = await _walletDbContext.Set<CashDepositState>()
            .SingleOrDefaultAsync(r => r.TransactionNo == transactionNo && r.CurrentState == state);

        if (transaction == null)
        {
            return false;
        }

        transaction.AccountCode = accountCode;
        await _walletDbContext.SaveChangesAsync();

        return true;
    }
}
