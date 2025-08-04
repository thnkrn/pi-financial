using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;

namespace Pi.BackofficeService.Application.Services.WalletService;

public record TransactionHistoryV2Filter(
    List<Product>? Product,
    string? TransactionNo,
    Status? Status,
    string? State,
    TransactionType? TransactionType,
    Channel? Channel,
    DateTime? CreatedAtFrom,
    DateTime? CreatedAtTo,
    DateTime? EffectiveDateFrom,
    DateTime? EffectiveDateTo,
    DateTime? PaymentReceivedFrom,
    DateTime? PaymentReceivedTo,
    string? CustomerCode,
    string? AccountCode,
    string? BankAccountNo,
    string? BankCode,
    string? BankName,
    string? AccountId,
    int? Page,
    int? PageSize,
    string? OrderBy,
    string? OrderDir);

public record TransferCashTransactionFilter(
    Status? Status,
    string? State,
    string? TransactionNo,
    string? TransferFromAccountCode,
    string? TransferToAccountCode,
    Product? TransferFromExchangeMarket,
    Product? TransferToExchangeMarket,
    DateTime? OtpConfirmedDateFrom,
    DateTime? OtpConfirmedDateTo,
    DateTime? CreatedAtFrom,
    DateTime? CreatedAtTo,
    int? Page,
    int? PageSize,
    string? OrderBy,
    string? OrderDir);

public interface ITransferCashService
{
    public Task<PaginateResponse<TransferCash>?> GetTransferCashHistory(TransferCashTransactionFilter filters, CancellationToken cancellationToken = default);
    public Task<TransferCash?> GetTransferCashByTransactionNo(string transactionNo, CancellationToken cancellationToken = default);
}