using System.Linq.Expressions;
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
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.Repositories;

public class GlobalWalletDepositRepository : IGlobalWalletDepositRepository
{
    private readonly WalletDbContext _walletDbContext;

    public GlobalWalletDepositRepository(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<GlobalWalletTransferState?> Get(string userId, Guid id)
    {
        var transaction = await _walletDbContext
            .Set<GlobalWalletTransferState>()
            .SingleOrDefaultAsync(t => t.CorrelationId == id && t.UserId == userId);

        return transaction;
    }

    public async Task<GlobalWalletTransferState?> Get(string userId, string transactionNo)
    {
        var transaction = await _walletDbContext
            .Set<GlobalWalletTransferState>()
            .SingleOrDefaultAsync(t => t.TransactionNo == transactionNo && t.UserId == userId);

        return transaction;
    }
    public async Task<List<GlobalWalletTransferState>> Get(int pageNum, int pageSize, string? orderBy, string? orderDir, GlobalTransferStateFilter? filters)
    {
        var query = _walletDbContext
            .Set<GlobalWalletTransferState>()
            .AsQueryable();

        query = _buildFiltersQuery(query, filters);

        if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(orderDir))
        {
            query = _buildOrderQuery(query, orderBy, orderDir);
        }

        var transactions = await query.Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return transactions;
    }

    public async Task<GlobalWalletTransferState?> GetByTransactionNo(string transactionNo)
    {
        var transaction = await _walletDbContext
            .Set<GlobalWalletTransferState>()
            .SingleOrDefaultAsync(t => t.TransactionNo == transactionNo);

        return transaction;
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

    public async Task<int> CountTransactionNoByDate(DateOnly date, TimeOnly cutOffTime, bool isThTime, string productInitial, Channel channel, TransactionType? transactionType)
    {
        var dateQuery = DateUtils.GetUtcStartEndDateTime(date, cutOffTime, isThTime);
        var count = await _walletDbContext.Set<GlobalWalletTransferState>()
            .Where(record => dateQuery.startDateTime < record.CreatedAt
                             && record.CreatedAt < dateQuery.endDateTime
                             && record.TransactionType == transactionType
                             && !string.IsNullOrWhiteSpace(record.TransactionNo)
                             && record.TransactionNo.Contains($"{productInitial}"))
            .CountAsync();

        return count;
    }

    public async Task UpdateTransactionNo(Guid correlationId, string transactionNo)
    {
        var result = await _walletDbContext.Set<GlobalWalletTransferState>()
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

    public async Task UpdateGlobalAccountByTransactionNo(string transactionNo, string globalAccount)
    {
        var result = await _walletDbContext.Set<GlobalWalletTransferState>()
            .SingleOrDefaultAsync(d => d.TransactionNo == transactionNo);

        if (result == null)
        {
            throw new KeyNotFoundException();
        }

        result.GlobalAccount = globalAccount;
        await _walletDbContext.SaveChangesAsync();
    }

    public async Task<bool> UpdateGlobalAccountByTransactionNoAndState(string transactionNo, string state, string globalAccount)
    {
        var result = await _walletDbContext.Set<GlobalWalletTransferState>()
            .SingleOrDefaultAsync(d => d.TransactionNo == transactionNo && d.CurrentState == state);

        if (result == null)
        {
            return false;
        }

        result.GlobalAccount = globalAccount;
        await _walletDbContext.SaveChangesAsync();
        return true;
    }

    public async Task UpdateRequestedFxAmountByTransactionNo(string transactionNo, decimal requestedFxAmount)
    {
        var result = await _walletDbContext.Set<GlobalWalletTransferState>()
            .SingleOrDefaultAsync(d => d.TransactionNo == transactionNo);

        if (result == null)
        {
            throw new KeyNotFoundException();
        }

        result.RequestedFxAmount = requestedFxAmount;
        await _walletDbContext.SaveChangesAsync();
    }

    public async Task<List<string>> GetFailedTransactionGlobalTransfer(string status)
    {
        var transactions = await _walletDbContext.Set<GlobalWalletTransferState>()
            .Where(t => t.CurrentState == status && t.TransactionNo != null)
            .Select(t => t.TransactionNo!).ToListAsync();

        return transactions;
    }

    public async Task<List<GlobalWalletTransferState>> GetListByDateTime(DateTime startDateTime, DateTime endDateTime)
    {
        return await _walletDbContext.Set<GlobalWalletTransferState>()
            .Where(g =>
                g.CreatedAt >= startDateTime && g.CreatedAt < endDateTime)
            .ToListAsync();
    }
    public async Task<int> CountTransactions(GlobalTransferStateFilter? filters)
    {
        var query = _walletDbContext
            .Set<GlobalWalletTransferState>()
            .AsQueryable();

        return await _buildFiltersQuery(query, filters).CountAsync();
    }

    private static IQueryable<GlobalWalletTransferState> _buildOrderQuery(
        IQueryable<GlobalWalletTransferState> query,
        string orderBy,
        string orderDir)
    {
        switch (orderBy)
        {
            case "CreatedAt":
                return _orderBy<DateTime>(query, orderBy, orderDir);
            default:
                return query;
        }
    }

    private static IQueryable<GlobalWalletTransferState> _orderBy<T>(IQueryable<GlobalWalletTransferState> query, string orderBy, string orderDir)
    {
        Expression<Func<GlobalWalletTransferState, T>> expression = q => EF.Property<T>(q, orderBy);

        return orderDir.ToLower() == "desc" ? query.OrderByDescending(expression) : query.OrderBy(expression);
    }

    private static IQueryable<GlobalWalletTransferState> _buildFiltersQuery(IQueryable<GlobalWalletTransferState> query, GlobalTransferStateFilter? filters)
    {
        return filters == null
            ? query
            : filters.GetExpressions().Aggregate(query, (current, expression) => current.Where(expression));
    }
}
