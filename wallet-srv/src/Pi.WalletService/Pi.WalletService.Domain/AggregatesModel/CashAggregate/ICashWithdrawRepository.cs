using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.CashAggregate;

public interface ICashWithdrawRepository : ITransactionRepository
{
    Task<CashWithdrawState?> Get(string transactionNo);
    Task<CashWithdrawState?> Get(string transactionNo, string currentState);
    Task<CashWithdrawState?> Get(string userId, Product product, string transactionNo);
    Task<List<CashWithdrawState>> Get(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<CashWithdrawState>? filters);

    /// <summary>
    /// For mocking test case on UAT only.
    /// </summary>
    Task<bool> UpdateAccountCodeByTransactionNoAndState(string transactionNo, string state, string bankAccountNo);
}
