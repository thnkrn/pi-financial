using Pi.WalletService.Application.Models;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Queries;

public record ActiveTransaction(
    Guid Id,
    string TransactionNo,
    Product Product,
    DateTime CreatedAt
);

public record TransactionFilters(
    Channel? Channel,
    Product? Product,
    string? State,
    string? BankCode,
    string? BankAccountNo,
    string? CustomerCode,
    string? AccountCode,
    string? TransactionNo,
    TransactionStatus? Status,
    DateTime? EffectiveDateFrom,
    DateTime? EffectiveDateTo,
    DateTime? CreatedAtFrom,
    DateTime? CreatedAtTo,
    TransactionType? TransactionType = null,
    string? UserId = null,
    string[]? NotStates = null,
    ProductType? ProductType = null,
    DateTime? PaymentReceivedFrom = null,
    DateTime? PaymentReceivedTo = null,
    string? BankName = null,
    string? ReferenceTransactionNo = null
);

public enum TransactionStatus
{
    Success,
    Pending,
    Fail
}

public enum Period
{
    FirstIntraDay,
    SecondIntraDay,
    AllDay
}

[Obsolete]
public interface ITransactionQueries
{
    Task<T> GetTransactionByTransactionNo<T>(
        string userId,
        string transactionNo,
        Product product) where T : Transaction;
    Task<List<ActiveTransaction>> GetActiveQrTransaction(
        string userId,
        Product product);
    Task<List<DepositTransaction>> GetDepositTransactions(
        PaginateRequest paginateRequest,
        TransactionFilters? filters);
    Task<List<WithdrawTransaction>> GetWithdrawTransactions(
        PaginateRequest paginateRequest,
        TransactionFilters? filters);
    Task<List<RefundTransaction>> GetRefundTransactions(
        PaginateRequest paginateRequest,
        TransactionFilters? filters);
    Task<List<T>> GetTransactions<T>(
        PaginateRequest paginateRequest,
        TransactionFilters? filters,
        string? sid = null,
        string? deviceId = null,
        string? device = null,
        string? accountId = null) where T : Transaction;
    Task<int> CountTransactions(TransactionFilters? filters);
    Task<int> CountDepositTransactions(TransactionFilters? filters);
    Task<int> CountWithdrawTransactions(TransactionFilters? filters);
    Task<int> CountRefundTransactions(TransactionFilters? filters);
    Task<T?> GetDepositTransactionByTransactionNo<T>(string transactionNo) where T : class;
    Task<T?> GetWithdrawTransactionByTransactionNo<T>(string transactionNo) where T : class;
    Task<CashDepositState?> GetCashDepositTransaction(string transactionNo);
    Task<DepositEntrypointState?> GetDepositEntrypointTransaction(string transactionNo);
    Task<WithdrawEntrypointState?> GetWithdrawEntrypointTransaction(string transactionNo);
    Task<CashWithdrawState?> GetCashWithdrawTransaction(string transactionNo);
    Task<List<string>> GetFailedTransactionsByProductAndStatus(Product product, string status);
}
