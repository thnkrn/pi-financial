using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;

public interface IWithdrawEntrypointRepository : IGenericRepository<WithdrawEntrypointState>, ITransactionRepository
{
    Task<WithdrawEntrypointState?> GetById(Guid correlationId);
    Task<WithdrawEntrypointState?> GetByTransactionNo(string transactionNo);
    new Task<List<WithdrawEntrypointTransaction>> GetTransactionListByDateTime(Product product, DateTime startDateTime, DateTime endDateTime);
    Task<bool> UpdateAccountCodeByTransactionNoAndUpBackState(string transactionNo, string upBackState, string accountCode);
    Task<bool> UpdateBankAccountNoByTransactionNoAndOddState(string transactionNo, string oddState, string bankAccountNo);

    Task<bool> UpdateAccountCodeByTransactionNoAndAtsState(string transactionNo, string atsState,
        string accountCode);
}
