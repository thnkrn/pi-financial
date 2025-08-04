using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Infrastructure.Extensions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using CashDepositState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashDepositState;
using DepositState = Pi.WalletService.Domain.AggregatesModel.DepositAggregate.DepositState;

namespace Pi.WalletService.Infrastructure.Repositories;

public class DepositRepository : IDepositRepository
{
    private readonly WalletDbContext _walletDbContext;

    public DepositRepository(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<List<DepositState>> GetActiveDeposit(string userId, Product product)
    {
        var transaction = await _walletDbContext.Set<DepositState>()
            .Where(t =>
                t.UserId == userId &&
                t.Product == product &&
                t.DepositQrGenerateDateTime != null &&
                t.DepositQrGenerateDateTime!.Value.AddMinutes(t.QrCodeExpiredTimeInMinute).CompareTo(DateTime.Now) >= 0 &&
                t.CurrentState == "DepositWaitingForPayment")
            .ToListAsync();

        return transaction;
    }

    public async Task<DepositState?> Get(string userId, Guid id)
    {
        var transaction = await _walletDbContext
            .Set<DepositState>()
            .SingleOrDefaultAsync(t => t.CorrelationId == id);

        return transaction;
    }

    public async Task<DepositState?> Get(string userId, string transactionNo)
    {
        var transaction = await _walletDbContext
            .Set<DepositState>()
            .SingleOrDefaultAsync(t => t.TransactionNo == transactionNo);

        return transaction;
    }

    public async Task<DepositState?> Get(Guid id)
    {
        var transaction = await _walletDbContext
            .Set<DepositState>()
            .SingleOrDefaultAsync(t => t.CorrelationId == id);

        return transaction;
    }

    public async Task<List<DepositState>> Get(
        int pageNum,
        int pageSize,
        string? orderBy,
        string? orderDir,
        IQueryFilter<DepositState>? filters)
    {
        var query = _walletDbContext
            .Set<DepositState>()
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

    public async Task<List<GlobalDepositTransaction>> GetGlobalDeposit(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<GlobalDepositTransaction>? filters)
    {
        var query = _walletDbContext
            .Set<DepositState>()
            .Where(q => q.Product == Product.GlobalEquities);

        if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(orderDir))
        {
            query = query.OrderByProperty(orderBy, orderDir);
        }
        else
        {
            query = query.OrderByDescending(q => q.CreatedAt);
        }

        var joinedQuery = query.Join(_walletDbContext.Set<GlobalWalletTransferState>(),
            state => state.TransactionNo,
            state => state.TransactionNo,
            ((depositState, transferState) => new GlobalDepositTransaction()
            {
                DepositState = depositState,
                GlobalWalletTransferState = transferState
            })
        ).WhereByFilters(filters);

        return await joinedQuery.Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<ThaiDepositTransaction>> GetThaiDeposit(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<ThaiDepositTransaction>? filters)
    {
        var query = _walletDbContext
            .Set<DepositState>()
            .Where(q => q.Product != Product.GlobalEquities);

        if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(orderDir))
        {
            query = query.OrderByProperty(orderBy, orderDir);
        }
        else
        {
            query = query.OrderByDescending(q => q.CreatedAt);
        }

        var joinedQuery = (from depositState in query
                           join cashDepositState in _walletDbContext.Set<CashDepositState>()
                               on depositState.TransactionNo equals cashDepositState.TransactionNo into joinedData
                           from cashDepositStateData in joinedData.DefaultIfEmpty()
                           select new ThaiDepositTransaction()
                           {
                               DepositState = depositState,
                               CashDepositState = cashDepositStateData
                           }).WhereByFilters(filters);

        return await joinedQuery.Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<DepositState?> GetByTransactionNo(string transactionNo)
    {
        var transaction = await _walletDbContext
            .Set<DepositState>()
            .SingleOrDefaultAsync(t => t.TransactionNo == transactionNo);

        return transaction;
    }

    public async Task<DepositState?> GetByTransactionNo(string userId, Product product, string transactionNo)
    {
        var transaction = await _walletDbContext
            .Set<DepositState>()
            .SingleOrDefaultAsync(t => t.TransactionNo == transactionNo && t.UserId == userId && t.Product == product);

        return transaction;
    }

    public async Task<List<DepositState>> GetListByDateTime(DateTime startDateTime, DateTime endDateTime)
    {
        return await _walletDbContext.Set<DepositState>()
            .Where(t => t.CreatedAt >= startDateTime && t.CreatedAt < endDateTime).ToListAsync();
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
        var count = await _walletDbContext.Set<DepositState>()
            .Where(record =>
                dateQuery.startDateTime < record.CreatedAt &&
                record.CreatedAt < dateQuery.endDateTime &&
                !string.IsNullOrWhiteSpace(record.TransactionNo) &&
                record.TransactionNo.Contains($"{productInitial}DP") &&
                record.Channel == channel)
            .CountAsync();

        return count;
    }

    public async Task<int> CountTransactions(IQueryFilter<DepositState>? filters)
    {
        return await _walletDbContext
            .Set<DepositState>()
            .AsQueryable()
            .WhereByFilters(filters)
            .CountAsync();
    }

    public async Task<int> CountGlobalTransactions(IQueryFilter<GlobalDepositTransaction>? filters)
    {
        return await _walletDbContext
            .Set<DepositState>()
            .Where(q => q.Product == Product.GlobalEquities)
            .Join(_walletDbContext.Set<GlobalWalletTransferState>(),
                state => state.TransactionNo,
                state => state.TransactionNo,
                ((depositState, transferState) => new GlobalDepositTransaction()
                {
                    DepositState = depositState,
                    GlobalWalletTransferState = transferState
                })
            )
            .WhereByFilters(filters)
            .CountAsync();
    }

    public async Task<int> CountThaiTransactions(IQueryFilter<ThaiDepositTransaction>? filters)
    {
        var query = from depositState in _walletDbContext.Set<DepositState>()
                    join cashDepositState in _walletDbContext.Set<CashDepositState>()
                        on depositState.TransactionNo equals cashDepositState.TransactionNo into joinedData
                    from cashDepositStateData in joinedData.DefaultIfEmpty()
                    select new ThaiDepositTransaction()
                    {
                        DepositState = depositState,
                        CashDepositState = cashDepositStateData
                    };

        return await query
            .Where(q => q.DepositState.Product != Product.GlobalEquities)
            .WhereByFilters(filters)
            .CountAsync();
    }

    public async Task UpdateTransactionNo(Guid correlationId, string transactionNo)
    {
        var result = await _walletDbContext.Set<DepositState>()
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
