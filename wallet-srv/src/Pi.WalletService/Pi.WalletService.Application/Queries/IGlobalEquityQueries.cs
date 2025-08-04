using Pi.WalletService.Application.Models;

namespace Pi.WalletService.Application.Queries;

public interface IGlobalEquityQueries
{
    Task<List<DepositTransaction>> GetDepositTransactions(
        PaginateRequest paginateRequest,
        TransactionFilters? filters);
    Task<List<WithdrawTransaction>> GetWithdrawTransactions(
        PaginateRequest paginateRequest,
        TransactionFilters? filters);
    Task<int> CountDepositTransactions(TransactionFilters? filters);
    Task<int> CountWithdrawTransactions(TransactionFilters? filters);
    Task<DepositTransaction?> GetDepositTransaction(string transactionNo);
    Task<WithdrawTransaction?> GetWithdrawTransaction(string transactionNo);
}
