using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Infrastructure.Extensions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using CashWithdrawState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashWithdrawState;

namespace Pi.WalletService.Infrastructure.Repositories;

public class CashWithdrawRepository : ICashWithdrawRepository
{
    private readonly WalletDbContext _walletDbContext;

    public CashWithdrawRepository(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public Task<Transaction?> GetByCorrelationId(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Transaction?> GetByNo(string transactionNo, Product? product, string? userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Transaction>> GetDepositTransactions(int pageNo, int pageSize, IQueryFilter<Transaction>? transactionFilters, string? orderBy, string? orderDir)
    {
        throw new NotImplementedException();
    }

    public Task<List<Transaction>> GetSetTradeEPayTransactions(int pageNum, int pageSize, IQueryFilter<CashDepositState>? transactionFilters, string? orderBy,
        string? orderDir)
    {
        throw new NotImplementedException();
    }

    public Task<List<Transaction>> GetWithdrawTransactions(int pageNo, int pageSize, IQueryFilter<Transaction>? transactionFilters, string? orderBy, string? orderDir)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountTransactions(TransactionType? type, IQueryFilter<Transaction>? depositFilter, IQueryFilter<Transaction>? withdrawFilter)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountSetTradeEPayTransactions(IQueryFilter<CashDepositState>? setTradeEPayFilter)
    {
        throw new NotImplementedException();
    }

    public Task<Guid?> GetIdByNo(string transactionNo)
    {
        throw new NotImplementedException();
    }

    public async Task<int> CountTransactionNoByDate(DateOnly date, TimeOnly cutOffTime, bool isThTime, string productInitial, Channel channel, TransactionType? transactionType = null)
    {
        var dateQuery = DateUtils.GetUtcStartEndDateTime(date, cutOffTime, isThTime);
        var count = await _walletDbContext.Set<CashWithdrawState>()
            .Where(record =>
                dateQuery.startDateTime < record.CreatedAt &&
                record.CreatedAt < dateQuery.endDateTime &&
                !string.IsNullOrWhiteSpace(record.TransactionNo) &&
                record.TransactionNo.Contains($"{productInitial}WS") &&
                record.Channel == channel)
            .CountAsync();

        return count;
    }

    public async Task<CashWithdrawState?> Get(string transactionNo)
    {
        var transaction = await _walletDbContext
            .Set<CashWithdrawState>()
            .SingleOrDefaultAsync(t => t.TransactionNo == transactionNo);

        return transaction;
    }

    public async Task<CashWithdrawState?> Get(string transactionNo, string currentState)
    {
        var transaction = await _walletDbContext
            .Set<CashWithdrawState>()
            .SingleOrDefaultAsync(t => t.TransactionNo == transactionNo && t.CurrentState == currentState);

        return transaction;
    }

    public async Task<CashWithdrawState?> Get(string userId, Product product, string transactionNo)
    {
        var transaction = await _walletDbContext
            .Set<CashWithdrawState>()
            .SingleOrDefaultAsync(t => t.TransactionNo == transactionNo && t.UserId == userId && t.Product == product);

        return transaction;
    }

    public async Task<List<CashWithdrawState>> Get(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<CashWithdrawState>? filters)
    {
        var query = _walletDbContext
            .Set<CashWithdrawState>()
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

    public async Task<bool> UpdateAccountCodeByTransactionNoAndState(string transactionNo, string state, string accountCode)
    {
        var transaction = await _walletDbContext.Set<CashWithdrawState>()
            .SingleOrDefaultAsync(r => r.TransactionNo == transactionNo && r.CurrentState == state);

        if (transaction == null)
        {
            return false;
        }

        transaction.AccountCode = accountCode;
        await _walletDbContext.SaveChangesAsync();

        return true;
    }

    public async Task UpdateTransactionNo(Guid correlationId, string transactionNo)
    {
        var result = await _walletDbContext.Set<CashWithdrawState>()
            .SingleOrDefaultAsync(d => d.CorrelationId == correlationId);

        if (result == null)
        {
            throw new KeyNotFoundException();
        }

        result.TransactionNo = transactionNo;

        try
        {
            await _walletDbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            if (ex.Message.ToLower().Contains("duplicate key"))
            {
                throw new DuplicateTransactionNoException();
            }
        }
    }

    public Task<List<Transaction>> GetTransactionListByDateTime(Product product, DateTime createdAtFrom,
        DateTime createdAtTo)
    {
        throw new NotImplementedException();
    }

    public Task<List<Transaction>> GetActiveQrTransaction(string userId, Product product)
    {
        throw new NotImplementedException();
    }
}
