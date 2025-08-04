#region

using Pi.WalletService.Application.Factories;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.Models;

#endregion

namespace Pi.WalletService.Application.Queries;

public class ThaiEquityQueries : IThaiEquityQueries
{
    private readonly IDepositRepository _depositRepository;
    private readonly IWithdrawRepository _withdrawRepository;
    private readonly ICashDepositRepository _cashDepositRepository;
    private readonly ICashWithdrawRepository _cashWithdrawRepository;

    public ThaiEquityQueries(IDepositRepository depositRepository,
        IWithdrawRepository withdrawRepository,
        ICashDepositRepository cashDepositRepository,
        ICashWithdrawRepository cashWithdrawRepository)
    {
        _depositRepository = depositRepository;
        _withdrawRepository = withdrawRepository;
        _cashDepositRepository = cashDepositRepository;
        _cashWithdrawRepository = cashWithdrawRepository;
    }

    public async Task<List<DepositTransaction>> GetDepositTransactions(
        PaginateRequest paginateRequest,
        TransactionFilters? filters)
    {
        if (filters is { Channel: Channel.SetTrade })
        {
            var queryFilters = EntityFactory.NewCashDepositFilters(filters);
            var records = await _cashDepositRepository.Get(
                paginateRequest.PageNo,
                paginateRequest.PageSize,
                paginateRequest.OrderBy,
                paginateRequest.OrderDirection,
                queryFilters);
            return records.Select(QueryFactory.NewDepositTransaction).ToList();
        }
        else
        {
            var queryFilters = filters != null ? EntityFactory.NewThaiDepositFilters(filters) : null;
            var records = await _depositRepository.GetThaiDeposit(
                paginateRequest.PageNo,
                paginateRequest.PageSize,
                paginateRequest.OrderBy,
                paginateRequest.OrderDirection,
                queryFilters);
            return records.Select(QueryFactory.NewDepositTransaction).ToList();
        }
    }

    public async Task<List<WithdrawTransaction>> GetWithdrawTransactions(
        PaginateRequest paginateRequest,
        TransactionFilters? filters)
    {
        var queryFilters = filters != null ? EntityFactory.NewThaiWithdrawFilters(filters) : null;
        var records = await _withdrawRepository.GetThaiWithdraw(
            paginateRequest.PageNo,
            paginateRequest.PageSize,
            paginateRequest.OrderBy,
            paginateRequest.OrderDirection,
            queryFilters);
        return records.Select(QueryFactory.NewWithdrawTransaction).ToList();
    }

    public async Task<int> CountDepositTransactions(TransactionFilters? filters)
    {
        if (filters is { Channel: Channel.SetTrade })
        {
            var queryFilters = EntityFactory.NewCashDepositFilters(filters);
            return await _cashDepositRepository.CountTransactions(queryFilters);
        }
        else
        {
            var queryFilters = filters != null ? EntityFactory.NewThaiDepositFilters(filters) : null;
            return await _depositRepository.CountThaiTransactions(queryFilters);
        }
    }

    public async Task<int> CountWithdrawTransactions(TransactionFilters? filters)
    {
        var queryFilters = filters != null ? EntityFactory.NewThaiWithdrawFilters(filters) : null;
        return await _withdrawRepository.CountThaiTransactions(queryFilters);
    }

    public async Task<DepositTransaction?> GetDepositTransaction(string transactionNo)
    {
        if (!transactionNo.ToUpper().StartsWith("DH"))
        {
            var cashDeposit = await _cashDepositRepository.Get(transactionNo);
            return cashDeposit != null ? QueryFactory.NewDepositTransaction(cashDeposit) : null;
        }

        var depositState = await _depositRepository.GetByTransactionNo(transactionNo);

        if (depositState == null) return null;

        var cashDepositState = await _cashDepositRepository.Get(transactionNo);

        return QueryFactory.NewDepositTransaction(new ThaiDepositTransaction()
        {
            DepositState = depositState,
            CashDepositState = cashDepositState
        });
    }

    public async Task<WithdrawTransaction?> GetWithdrawTransaction(string transactionNo)
    {
        var withdrawState = await _withdrawRepository.GetByTransactionNo(transactionNo);

        if (withdrawState == null) return null;

        var cashWithdraw = await _cashWithdrawRepository.Get(transactionNo);

        if (cashWithdraw == null) return null;

        return QueryFactory.NewWithdrawTransaction(new ThaiWithdrawTransaction()
        {
            WithdrawState = withdrawState,
            CashWithdrawState = cashWithdraw
        });
    }
}
