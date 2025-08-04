using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.DepositAggregate;

public interface IDepositRepository : ITransactionRepository
{
    Task<List<DepositState>> GetActiveDeposit(string userId, Product product);
    Task<DepositState?> Get(string userId, Guid id);
    Task<DepositState?> Get(string userId, string transactionNo);
    Task<DepositState?> Get(Guid id);
    Task<List<DepositState>> Get(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<DepositState>? filters);
    Task<List<GlobalDepositTransaction>> GetGlobalDeposit(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<GlobalDepositTransaction>? filters);
    Task<List<ThaiDepositTransaction>> GetThaiDeposit(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<ThaiDepositTransaction>? filters);
    Task<DepositState?> GetByTransactionNo(string transactionNo);
    Task<DepositState?> GetByTransactionNo(string userId, Product product, string transactionNo);
    Task<List<DepositState>> GetListByDateTime(DateTime startDateTime, DateTime endDateTime);
    Task<int> CountTransactions(IQueryFilter<DepositState>? filters);
    Task<int> CountGlobalTransactions(IQueryFilter<GlobalDepositTransaction>? filters);
    Task<int> CountThaiTransactions(IQueryFilter<ThaiDepositTransaction>? filters);
}
