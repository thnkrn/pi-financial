using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.AtsWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.OddWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.RecoveryAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using WithdrawEntrypointMachineState = Pi.WalletService.IntegrationEvents.Models.WithdrawEntrypointState;

namespace Pi.WalletService.Infrastructure.Repositories;

public class WithdrawEntrypointRepository : GenericRepository<WithdrawEntrypointState>, IWithdrawEntrypointRepository
{
    private readonly WalletDbContext _walletDbContext;
    private readonly DbSet<WithdrawEntrypointState> _entitySet;

    private readonly ILogger<WithdrawEntrypointRepository> _logger;

    public WithdrawEntrypointRepository(WalletDbContext walletDbContext, ILogger<WithdrawEntrypointRepository> logger) : base(walletDbContext)
    {
        _walletDbContext = walletDbContext;
        _entitySet = _walletDbContext.Set<WithdrawEntrypointState>();

        _logger = logger;
    }

    public async Task<WithdrawEntrypointState?> GetById(Guid correlationId)
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

    public async Task<WithdrawEntrypointState?> GetByTransactionNo(string transactionNo)
    {
        return await _entitySet.SingleOrDefaultAsync(t => t.TransactionNo == transactionNo);
    }

    public async Task<List<WithdrawEntrypointTransaction>> GetTransactionListByDateTime(Product product, DateTime startDateTime, DateTime endDateTime)
    {
        var transaction = from withdrawEntrypoint in _walletDbContext.Set<WithdrawEntrypointState>()
                          join oddWithdraw in _walletDbContext.Set<OddWithdrawState>() on withdrawEntrypoint.CorrelationId equals oddWithdraw.CorrelationId into oddWithdrawGroup
                          from oddWithdraw in oddWithdrawGroup.DefaultIfEmpty()
                          join upBack in _walletDbContext.Set<UpBackState>() on withdrawEntrypoint.CorrelationId equals upBack.CorrelationId into upBackGroup
                          from upBack in upBackGroup.DefaultIfEmpty()
                          join globalTransfer in _walletDbContext.Set<GlobalTransferState>() on withdrawEntrypoint.CorrelationId equals globalTransfer.CorrelationId into globalTransferGroup
                          from globalTransfer in globalTransferGroup.DefaultIfEmpty()
                          join recovery in _walletDbContext.Set<RecoveryState>() on withdrawEntrypoint.CorrelationId equals recovery.CorrelationId into recoveryGroup
                          from recovery in recoveryGroup.DefaultIfEmpty()
                          where withdrawEntrypoint.Product == product &&
                                withdrawEntrypoint.CreatedAt >= startDateTime &&
                                withdrawEntrypoint.CreatedAt <= endDateTime
                          select new WithdrawEntrypointTransaction
                          {
                              WithdrawEntrypoint = withdrawEntrypoint,
                              OddWithdraw = oddWithdraw,
                              UpBack = upBack,
                              GlobalTransfer = globalTransfer,
                              Recovery = recovery
                          };

        return await transaction.ToListAsync();

    }

    public Task<List<Transaction>> GetActiveQrTransaction(string userId, Product product)
    {
        throw new NotImplementedException();
    }

    public async Task<int> CountTransactionNoByDate(DateOnly date, TimeOnly cutOffTime, bool isThTime, string productInitial, Channel channel, TransactionType? transactionType = null)
    {
        var utcDateTime = DateUtils.GetUtcStartEndDateTime(date, cutOffTime, isThTime);
        var count = await _entitySet
            .Where(r =>
                utcDateTime.startDateTime < r.CreatedAt &&
                utcDateTime.endDateTime > r.CreatedAt &&
                !string.IsNullOrWhiteSpace(r.TransactionNo) &&
                r.TransactionNo.Contains($"{productInitial}WS") &&
                r.Channel == channel)
            .CountAsync();

        return count;
    }

    public async Task UpdateTransactionNo(Guid correlationId, string transactionNo)
    {
        var transaction = await _entitySet.SingleOrDefaultAsync(r => r.CorrelationId == correlationId);

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
        var result = await (from withdrawEntrypoint in _walletDbContext.Set<WithdrawEntrypointState>()
            .Where(r => r.TransactionNo == transactionNo && r.CurrentState == WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.UpBackProcessing))
            .DefaultIfEmpty()
                            from upBack in _walletDbContext.Set<UpBackState>()
                            .Where(r => withdrawEntrypoint != null && r.CorrelationId == withdrawEntrypoint.CorrelationId && r.CurrentState == upBackState)
                            .DefaultIfEmpty()
                            select withdrawEntrypoint).SingleOrDefaultAsync();

        if (result == null)
        {
            return false;
        }

        result.AccountCode = accountCode;
        await _walletDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateBankAccountNoByTransactionNoAndOddState(string transactionNo, string oddState,
        string bankAccountNo)
    {
        var result = await (from withdrawEntrypoint in _walletDbContext.Set<WithdrawEntrypointState>()
                .Where(r => r.TransactionNo == transactionNo && r.CurrentState ==
                    WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.WithdrawProcessing))
                .DefaultIfEmpty()
                            from oddWithdraw in _walletDbContext.Set<OddWithdrawState>()
                                .Where(r => withdrawEntrypoint != null && r.CorrelationId == withdrawEntrypoint.CorrelationId &&
                                            r.CurrentState == oddState)
                                .DefaultIfEmpty()
                            select withdrawEntrypoint).SingleOrDefaultAsync();

        if (result == null)
        {
            return false;
        }

        result.BankAccountNo = bankAccountNo;
        await _walletDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAccountCodeByTransactionNoAndAtsState(string transactionNo, string atsState, string accountCode)
    {
        var result = await (from withdrawEntrypoint in _walletDbContext.Set<WithdrawEntrypointState>()
                .Where(r => r.TransactionNo == transactionNo && r.CurrentState ==
                    WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.WithdrawProcessing))
                .DefaultIfEmpty()
                            from atsWithdraw in _walletDbContext.Set<AtsWithdrawState>()
                                .Where(r => withdrawEntrypoint != null && r.CorrelationId == withdrawEntrypoint.CorrelationId &&
                                            r.CurrentState == atsState)
                                .DefaultIfEmpty()
                            select withdrawEntrypoint).SingleOrDefaultAsync();

        if (result == null)
        {
            return false;
        }

        result.AccountCode = accountCode;
        await _walletDbContext.SaveChangesAsync();
        return true;
    }
}
