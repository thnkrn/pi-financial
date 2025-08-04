using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;

namespace Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;

public interface IGlobalWalletDepositRepository : ITransactionRepository
{
    Task<GlobalWalletTransferState?> Get(string userId, Guid id);
    Task<GlobalWalletTransferState?> Get(string userId, string transactionNo);
    Task<List<GlobalWalletTransferState>> Get(int pageNum, int pageSize, string? orderBy, string? orderDir, GlobalTransferStateFilter? filters);
    Task<GlobalWalletTransferState?> GetByTransactionNo(string transactionNo);
    Task<List<string>> GetFailedTransactionGlobalTransfer(string status);
    Task<List<GlobalWalletTransferState>> GetListByDateTime(DateTime startDateTime, DateTime endDateTime);
    Task<int> CountTransactions(GlobalTransferStateFilter? filters);

    /// <summary>
    /// For mocking test case on UAT only.
    /// </summary>
    Task UpdateRequestedFxAmountByTransactionNo(string transactionNo, decimal requestedFxAmount);

    /// <summary>
    /// For mocking test case on UAT only.
    /// </summary>
    Task UpdateGlobalAccountByTransactionNo(string transactionNo, string globalAccount);

    /// <summary>
    /// For mocking test case on UAT only.
    /// </summary>
    Task<bool> UpdateGlobalAccountByTransactionNoAndState(string transactionNo, string state, string globalAccount);

}
