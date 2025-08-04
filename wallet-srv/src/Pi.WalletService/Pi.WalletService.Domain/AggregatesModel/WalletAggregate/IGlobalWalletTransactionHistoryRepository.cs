using Pi.Common.SeedWork;

namespace Pi.WalletService.Domain.AggregatesModel.WalletAggregate;

public interface IGlobalWalletTransactionHistoryRepository : IRepository<GlobalWalletTransactionHistory>
{
    Task<GlobalWalletTransactionHistory> Get(Guid id);
    Task<GlobalWalletTransactionHistory?> GetByTransactionNo(string transactionNo, string userId);
    GlobalWalletTransactionHistory Create(GlobalWalletTransactionHistory globalWalletTransactionHistory);
}