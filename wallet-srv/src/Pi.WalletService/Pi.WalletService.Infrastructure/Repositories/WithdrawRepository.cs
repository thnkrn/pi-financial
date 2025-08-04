using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Infrastructure.Extensions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.Repositories;

public class WithdrawRepository : IWithdrawRepository
{
    private readonly WalletDbContext _walletDbContext;

    public WithdrawRepository(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<WithdrawState?> Get(string userId, string transactionNo)
    {
        var transaction = await _walletDbContext
            .Set<WithdrawState>()
            .SingleOrDefaultAsync(t => t.TransactionNo == transactionNo);

        return transaction;
    }

    public async Task<List<WithdrawState>> Get(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<WithdrawState>? filters)
    {
        var query = _walletDbContext
            .Set<WithdrawState>()
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

        var transactions = await query.Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return transactions;
    }

    public async Task<List<GlobalWithdrawTransaction>> GetGlobalWithdraw(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<GlobalWithdrawTransaction>? filters)
    {
        var query = _walletDbContext
            .Set<WithdrawState>()
            .AsQueryable()
            .Where(q => q.Product == Product.GlobalEquities);

        if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(orderDir))
        {
            query = query.OrderByProperty(orderBy, orderDir);
        }
        else
        {
            query = query.OrderByDescending(q => q.CreatedAt);
        }

        var joinedQuery = query
            .Join(_walletDbContext.Set<GlobalWalletTransferState>(),
                state => state.TransactionNo,
                state => state.TransactionNo,
                ((withdrawState, transferState) => new GlobalWithdrawTransaction
                {
                    WithdrawState = withdrawState,
                    GlobalWalletTransferState = transferState
                })
            )
            .WhereByFilters(filters);

        return await joinedQuery.Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<ThaiWithdrawTransaction>> GetThaiWithdraw(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<ThaiWithdrawTransaction>? filters)
    {
        var query = _walletDbContext
            .Set<WithdrawState>()
            .AsQueryable()
            .Where(q => q.Product != Product.GlobalEquities);

        if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(orderDir))
        {
            query = query.OrderByProperty(orderBy, orderDir);
        }
        else
        {
            query = query.OrderByDescending(q => q.CreatedAt);
        }

        var joinedQuery = query
            .Join(_walletDbContext.Set<CashWithdrawState>(),
                state => state.TransactionNo,
                state => state.TransactionNo,
                ((withdrawState, cashWithdrawState) => new ThaiWithdrawTransaction
                {
                    WithdrawState = withdrawState,
                    CashWithdrawState = cashWithdrawState
                })
            )
            .WhereByFilters(filters);

        return await joinedQuery.Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<WithdrawState?> GetByTransactionNo(string userId, Product product, string transactionNo)
    {
        var transaction = await _walletDbContext
            .Set<WithdrawState>()
            .SingleOrDefaultAsync(t => t.TransactionNo == transactionNo && t.UserId == userId && t.Product == product);

        return transaction;
    }

    public async Task<WithdrawState?> GetByTransactionNo(string transactionNo)
    {
        var transaction = await _walletDbContext
            .Set<WithdrawState>()
            .SingleOrDefaultAsync(t => t.TransactionNo == transactionNo);

        return transaction;
    }

    public async Task<List<WithdrawState>> GetListByDateTime(DateTime startDateTime, DateTime endDateTime)
    {
        return await _walletDbContext.Set<WithdrawState>()
            .Where(t => t.CreatedAt >= startDateTime && t.CreatedAt < endDateTime).ToListAsync();
    }
    public async Task<int> CountTransactions(IQueryFilter<WithdrawState>? filters)
    {
        return await _walletDbContext
            .Set<WithdrawState>()
            .AsQueryable()
            .WhereByFilters(filters)
            .CountAsync();
    }

    public async Task<int> CountGlobalTransactions(IQueryFilter<GlobalWithdrawTransaction>? filters)
    {
        return await _walletDbContext
            .Set<WithdrawState>()
            .AsQueryable()
            .Where(q => q.Product == Product.GlobalEquities)
            .Join(_walletDbContext.Set<GlobalWalletTransferState>(),
                state => state.TransactionNo,
                state => state.TransactionNo,
                ((withdrawState, transferState) => new GlobalWithdrawTransaction()
                {
                    WithdrawState = withdrawState,
                    GlobalWalletTransferState = transferState
                })
            )
            .WhereByFilters(filters)
            .CountAsync();
    }

    public async Task<int> CountThaiTransactions(IQueryFilter<ThaiWithdrawTransaction>? filters)
    {
        return await _walletDbContext
            .Set<WithdrawState>()
            .AsQueryable()
            .Where(q => q.Product != Product.GlobalEquities)
            .Join(_walletDbContext.Set<CashWithdrawState>(),
                state => state.TransactionNo,
                state => state.TransactionNo,
                ((withdrawState, cashWithdrawState) => new ThaiWithdrawTransaction()
                {
                    WithdrawState = withdrawState,
                    CashWithdrawState = cashWithdrawState
                })
            )
            .WhereByFilters(filters)
            .CountAsync();
    }

    public async Task<bool> UpdateBankAccountByTransactionNoAndState(string transactionNo, string state, string bankAccountNo)
    {
        var transaction = await _walletDbContext.Set<WithdrawState>()
            .SingleOrDefaultAsync(r => r.TransactionNo == transactionNo && r.CurrentState == state);

        if (transaction == null)
        {
            return false;
        }

        transaction.BankAccountNo = bankAccountNo;
        await _walletDbContext.SaveChangesAsync();

        return true;
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
        var count = await _walletDbContext.Set<WithdrawState>()
            .Where(record =>
                dateQuery.startDateTime < record.CreatedAt &&
                record.CreatedAt < dateQuery.endDateTime &&
                !string.IsNullOrWhiteSpace(record.TransactionNo) &&
                record.Channel == channel)
            .CountAsync();

        return count;
    }

    public async Task UpdateTransactionNo(Guid correlationId, string transactionNo)
    {
        var result = await _walletDbContext.Set<WithdrawState>()
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
