using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;

public interface IWithdrawRepository : ITransactionRepository
{
    Task<WithdrawState?> Get(string userId, string transactionNo);
    Task<List<WithdrawState>> Get(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<WithdrawState>? filters);
    Task<List<GlobalWithdrawTransaction>> GetGlobalWithdraw(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<GlobalWithdrawTransaction>? filters);
    Task<List<ThaiWithdrawTransaction>> GetThaiWithdraw(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<ThaiWithdrawTransaction>? filters);
    Task<WithdrawState?> GetByTransactionNo(string transactionNo);
    Task<WithdrawState?> GetByTransactionNo(string userId, Product product, string transactionNo);
    Task<List<WithdrawState>> GetListByDateTime(DateTime startDateTime, DateTime endDateTime);
    Task<int> CountTransactions(IQueryFilter<WithdrawState>? filters);
    Task<int> CountGlobalTransactions(IQueryFilter<GlobalWithdrawTransaction>? filters);
    Task<int> CountThaiTransactions(IQueryFilter<ThaiWithdrawTransaction>? filters);
    Task<bool> UpdateBankAccountByTransactionNoAndState(string transactionNo, string state, string bankAccountNo);
}
