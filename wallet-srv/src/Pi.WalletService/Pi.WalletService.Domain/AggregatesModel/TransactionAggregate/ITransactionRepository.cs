using System.Runtime.Serialization;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;

[Serializable]
public class DuplicateTransactionNoException : Exception
{
    public DuplicateTransactionNoException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public DuplicateTransactionNoException()
    {
    }

    public DuplicateTransactionNoException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected DuplicateTransactionNoException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

public interface ITransactionRepository
{
    Task<Transaction?> GetByCorrelationId(Guid correlationId);
    Task<Transaction?> GetByNo(string transactionNo, Product? product, string? userId);
    Task<List<Transaction>> GetDepositTransactions(int pageNum, int pageSize, IQueryFilter<Transaction>? transactionFilters, string? orderBy, string? orderDir);
    Task<List<Transaction>> GetSetTradeEPayTransactions(int pageNum, int pageSize, IQueryFilter<CashDepositState>? transactionFilters, string? orderBy, string? orderDir);
    Task<List<Transaction>> GetWithdrawTransactions(int pageNum, int pageSize, IQueryFilter<Transaction>? transactionFilters, string? orderBy, string? orderDir);
    Task<int> CountTransactions(TransactionType? type, IQueryFilter<Transaction>? depositFilter, IQueryFilter<Transaction>? withdrawFilter);
    Task<int> CountSetTradeEPayTransactions(IQueryFilter<CashDepositState>? setTradeEPayFilter);
    Task<Guid?> GetIdByNo(string transactionNo);
    Task<int> CountTransactionNoByDate(DateOnly date, TimeOnly cutOffTime, bool isThTime, string productInitial, Channel channel, TransactionType? transactionType = null);
    Task UpdateTransactionNo(Guid correlationId, string transactionNo);
    Task<List<Transaction>> GetTransactionListByDateTime(Product product, DateTime createdAtFrom, DateTime createdAtTo);
    Task<List<Transaction>> GetActiveQrTransaction(string userId, Product product);
}
