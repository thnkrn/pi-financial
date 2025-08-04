using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;

public interface IDepositEntrypointRepository : IGenericRepository<DepositEntrypointState>, ITransactionRepository
{
    Task<DepositEntrypointState?> GetById(Guid correlationId);
    Task<DepositEntrypointState?> GetByTransactionNo(string transactionNo);
    new Task<List<DepositEntrypointTransaction>> GetTransactionListByDateTime(Product product, DateTime startDateTime, DateTime endDateTime);
    Task UpdatePaymentReceivedData(DepositEntrypointState updated);
    Task<bool> UpdateAccountCodeByTransactionNoAndUpBackState(string transactionNo, string upBackState, string accountCode);
}
