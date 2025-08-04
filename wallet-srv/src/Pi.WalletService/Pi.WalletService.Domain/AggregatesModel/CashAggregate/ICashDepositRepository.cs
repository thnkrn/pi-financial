using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.CashAggregate;

public interface ICashDepositRepository
{
    Task<CashDepositState?> Get(string transactionNo);
    Task<CashDepositState?> Get(string userId, Product product, string transactionNo);

    Task<List<CashDepositState>> Get(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<CashDepositState>? filters);
    Task<int> CountTransactions(IQueryFilter<CashDepositState>? filters);

    /// <summary>
    /// For mocking test case on UAT only.
    /// </summary>
    Task<bool> UpdateAccountCodeByTransactionNoAndState(string transactionNo, string state, string bankAccountNo);
}
