using Pi.WalletService.Application.Models;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Transaction = Pi.WalletService.Domain.AggregatesModel.TransactionAggregate.Transaction;

namespace Pi.WalletService.Application.Queries;

public record TransactionFilterV2(
    Status? Status,
    Product[]? Product,
    Channel? Channel,
    string? TransactionNo,
    string? CustomerCode,
    string? AccountCode,
    string? BankAccountNo,
    string? BankCode,
    string? BankName,
    string? State,
    DateTime? CreatedAtFrom,
    DateTime? CreatedAtTo,
    DateTime? EffectiveDateFrom,
    DateTime? EffectiveDateTo,
    DateTime? PaymentReceivedFrom,
    DateTime? PaymentReceivedTo,
    TransactionType? TransactionType = null,
    string? UserId = null,
    string[]? NotStates = null
);

public interface ITransactionQueriesV2
{
    Task<List<ActiveTransaction>> GetActiveQrTransaction(
        string userId,
        Product product);
    Task<Transaction?> GetTransactionById(Guid correlationId);
    Task<Transaction?> GetTransactionByTransactionNo(string transactionNo, string? excludeInnerState);
    Task<Transaction?> GetTransactionByTransactionNo(string transactionNo, Product product, string? userId);
    Task<(TransactionSummary, List<Transaction>)> GetTransactionsSummaryByDate(Product product, DateTime createdAtFrom, DateTime createdAtTo);
    Task<List<Transaction>> GetTransactions(
        PaginateRequest paginateRequest,
        TransactionFilterV2? filters,
        string? sid = null,
        string? deviceId = null,
        string? device = null,
        string? accountId = null);

    Task<int> CountTransactions(TransactionFilterV2? filters);
    Task<List<EmailHistory>> GetTransactionEmailHistory(string transactionNo);
}