using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.RefundInfoAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
using Pi.WalletService.Infrastructure.Extensions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using DepositEntrypointState = Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate.DepositEntrypointState;
using QrDepositState = Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate.QrDepositState;

namespace Pi.WalletService.Infrastructure.Repositories;

public class DepositEntrypointRepository : GenericRepository<DepositEntrypointState>, IDepositEntrypointRepository
{
    private readonly WalletDbContext _walletDbContext;
    private readonly DbSet<DepositEntrypointState> _entitySet;

    public DepositEntrypointRepository(WalletDbContext walletDbContext) : base(walletDbContext)
    {
        _walletDbContext = walletDbContext;
        _entitySet = _walletDbContext.Set<DepositEntrypointState>();
    }

    public async Task<DepositEntrypointState?> GetById(Guid correlationId)
    {
        return await _entitySet.SingleOrDefaultAsync(r => r.CorrelationId == correlationId);
    }

    Task<Transaction?> ITransactionRepository.GetByCorrelationId(Guid correlationId)
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

    public async Task<Guid?> GetIdByNo(string transactionNo)
    {
        return await _entitySet
            .Where(d => d.TransactionNo == transactionNo)
            .Select(d => d.CorrelationId).SingleOrDefaultAsync();
    }

    public async Task<DepositEntrypointState?> GetByTransactionNo(string transactionNo)
    {
        return await _entitySet.SingleOrDefaultAsync(t => t.TransactionNo == transactionNo);
    }

    public async Task<int> CountTransactions(IQueryFilter<DepositEntrypointState>? filters)
    {
        return await _entitySet.WhereByFilters(filters).DefaultIfEmpty().CountAsync();
    }

    public async Task<List<DepositEntrypointTransaction>> GetTransactionListByDateTime(Product product, DateTime startDateTime, DateTime endDateTime)
    {
        var transaction = from depositEntrypoint in _entitySet
                          join qrDeposit in _walletDbContext.Set<QrDepositState>() on depositEntrypoint.CorrelationId equals qrDeposit.CorrelationId into qrDepositGroup
                          from qrDeposit in qrDepositGroup.DefaultIfEmpty()
                          join oddDeposit in _walletDbContext.Set<OddDepositState>() on depositEntrypoint.CorrelationId equals oddDeposit.CorrelationId into oddDepositGroup
                          from oddDeposit in oddDepositGroup.DefaultIfEmpty()
                          join upBack in _walletDbContext.Set<UpBackState>() on depositEntrypoint.CorrelationId equals upBack.CorrelationId into upBackGroup
                          from upBack in upBackGroup.DefaultIfEmpty()
                          join globalTransfer in _walletDbContext.Set<GlobalTransferState>() on depositEntrypoint.CorrelationId equals globalTransfer.CorrelationId into globalTransferGroup
                          from globalTransfer in globalTransferGroup.DefaultIfEmpty()
                          join refundInfo in _walletDbContext.Set<RefundInfo>() on depositEntrypoint.CorrelationId equals refundInfo.TicketId into refundGroup
                          from refundInfo in refundGroup.Where(r => r.CurrentState != RefundStatus.RefundFailed.ToString()).OrderByDescending(r => r.CreatedAt).Take(1).DefaultIfEmpty()
                          where depositEntrypoint.Product == product &&
                                depositEntrypoint.CreatedAt >= startDateTime &&
                                depositEntrypoint.CreatedAt <= endDateTime
                          select new DepositEntrypointTransaction
                          {
                              DepositEntrypoint = depositEntrypoint,
                              QrDeposit = qrDeposit,
                              OddDeposit = oddDeposit,
                              UpBack = upBack,
                              GlobalTransfer = globalTransfer,
                              RefundInfo = refundInfo
                          };

        return await transaction.ToListAsync();

    }

    public Task<List<Transaction>> GetActiveQrTransaction(string userId, Product product)
    {
        throw new NotImplementedException();
    }

    public async Task UpdatePaymentReceivedData(DepositEntrypointState updated)
    {
        var existingState = await _entitySet.SingleOrDefaultAsync(r => r.CorrelationId == updated.CorrelationId);

        if (existingState == null)
        {
            throw new KeyNotFoundException();
        }

        existingState.BankName = updated.BankName;
        existingState.BankCode = updated.BankCode;
        existingState.BankAccountName = updated.BankAccountName;
        existingState.BankAccountNo = updated.BankAccountNo;
        existingState.NetAmount = updated.NetAmount;

        await _walletDbContext.SaveChangesAsync();
    }

    public async Task<int> CountTransactionNoByDate(DateOnly date, TimeOnly cutOffTime, bool isThTime, string productInitial, Channel channel, TransactionType? transactionType = null)
    {
        var dateQuery = DateUtils.GetUtcStartEndDateTime(date, cutOffTime, isThTime);
        var count = await _entitySet
            .Where(record =>
                dateQuery.startDateTime < record.CreatedAt &&
                record.CreatedAt < dateQuery.endDateTime &&
                !string.IsNullOrWhiteSpace(record.TransactionNo) &&
                record.TransactionNo.Contains($"{productInitial}DP") &&
                record.Channel == channel)
            .CountAsync();

        return count;
    }

    public async Task UpdateTransactionNo(Guid correlationId, string transactionNo)
    {
        var transaction = await _entitySet.Where(r => r.CorrelationId == correlationId).SingleOrDefaultAsync();

        if (transaction == null)
        {
            throw new KeyNotFoundException();
        }

        transaction.TransactionNo = transactionNo;

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

    Task<List<Transaction>> ITransactionRepository.GetTransactionListByDateTime(Product product,
        DateTime createdAtFrom, DateTime createdAtTo)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAccountCodeByTransactionNoAndUpBackState(string transactionNo, string upBackState, string accountCode)
    {
        var result = await (from depositEntrypoint in _walletDbContext.Set<DepositEntrypointState>()
                            join upBack in _walletDbContext.Set<UpBackState>() on depositEntrypoint.CorrelationId equals upBack.CorrelationId into upBackGroup
                            from upBack in upBackGroup.DefaultIfEmpty()
                            where depositEntrypoint.TransactionNo == transactionNo && upBack.CurrentState == upBackState
                            select depositEntrypoint).SingleOrDefaultAsync();

        if (result == null)
        {
            return false;
        }

        result.AccountCode = accountCode;
        await _walletDbContext.SaveChangesAsync();
        return true;
    }
}
