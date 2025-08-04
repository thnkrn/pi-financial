using Pi.Common.SeedWork;
namespace Pi.WalletService.Domain.AggregatesModel.WalletAggregate;

public interface ITransactionHistoryRepository : IRepository<TransactionHistory>
{
    TransactionHistory Create(TransactionHistory transactionHistory);
}