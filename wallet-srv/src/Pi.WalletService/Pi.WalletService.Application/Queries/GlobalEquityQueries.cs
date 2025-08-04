using Pi.WalletService.Application.Factories;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries.Filters;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;

namespace Pi.WalletService.Application.Queries;

public class GlobalEquityQueries : IGlobalEquityQueries
{
    private readonly IDepositRepository _depositRepository;
    private readonly IWithdrawRepository _withdrawRepository;
    private readonly IGlobalWalletDepositRepository _globalWalletDepositRepository;

    public GlobalEquityQueries(
        IDepositRepository depositRepository,
        IWithdrawRepository withdrawRepository,
        IGlobalWalletDepositRepository globalWalletDepositRepository
        )
    {
        _depositRepository = depositRepository;
        _withdrawRepository = withdrawRepository;
        _globalWalletDepositRepository = globalWalletDepositRepository;
    }

    public async Task<List<DepositTransaction>> GetDepositTransactions(
        PaginateRequest paginateRequest,
        TransactionFilters? filters)
    {
        var queryFilters = filters != null ? EntityFactory.NewGlobalDepositFilters(filters) : null;
        var records = await _depositRepository.GetGlobalDeposit(
            paginateRequest.PageNo,
            paginateRequest.PageSize,
            paginateRequest.OrderBy,
            paginateRequest.OrderDirection,
            queryFilters);
        return records.Select(QueryFactory.NewDepositTransaction).ToList();
    }

    public async Task<List<WithdrawTransaction>> GetWithdrawTransactions(
        PaginateRequest paginateRequest,
        TransactionFilters? filters)
    {
        var queryFilters = filters != null ? EntityFactory.NewTransactionFilters<GlobalWithdrawFilters>(filters) : null;
        var records = await _withdrawRepository.GetGlobalWithdraw(
            paginateRequest.PageNo,
            paginateRequest.PageSize,
            paginateRequest.OrderBy,
            paginateRequest.OrderDirection,
            queryFilters);
        return records.Select(QueryFactory.NewWithdrawTransaction).ToList();
    }

    public async Task<int> CountDepositTransactions(TransactionFilters? filters)
    {
        var queryFilters = filters != null ? EntityFactory.NewGlobalDepositFilters(filters) : null;
        return await _depositRepository.CountGlobalTransactions(queryFilters);
    }

    public async Task<int> CountWithdrawTransactions(TransactionFilters? filters)
    {
        var queryFilters = filters != null ? EntityFactory.NewTransactionFilters<GlobalWithdrawFilters>(filters) : null;
        return await _withdrawRepository.CountGlobalTransactions(queryFilters);
    }

    public async Task<DepositTransaction?> GetDepositTransaction(string transactionNo)
    {
        var depositState = await _depositRepository.GetByTransactionNo(transactionNo);

        if (depositState == null) return null;

        var globalTransfer = await _globalWalletDepositRepository.GetByTransactionNo(transactionNo);

        if (globalTransfer == null) return null;

        return QueryFactory.NewDepositTransaction(new GlobalDepositTransaction
        {
            DepositState = depositState,
            GlobalWalletTransferState = globalTransfer
        });
    }

    public async Task<WithdrawTransaction?> GetWithdrawTransaction(string transactionNo)
    {

        var withdrawState = await _withdrawRepository.GetByTransactionNo(transactionNo);

        if (withdrawState == null) return null;

        var globalTransfer = await _globalWalletDepositRepository.GetByTransactionNo(transactionNo);

        if (globalTransfer == null) return null;

        return QueryFactory.NewWithdrawTransaction(new GlobalWithdrawTransaction()
        {
            WithdrawState = withdrawState,
            GlobalWalletTransferState = globalTransfer
        });
    }
}
