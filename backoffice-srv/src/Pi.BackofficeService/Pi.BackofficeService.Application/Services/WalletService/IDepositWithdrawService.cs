using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Application.Services.WalletService;

public record TransactionFilterRequest(
    Channel? Channel,
    Product? Product,
    ProductType? ProductType,
    string? State,
    string? BankCode,
    string? BankName,
    string? AccountNumber,
    string? CustomerCode,
    string? AccountCode,
    string? TransactionNumber,
    string? Status,
    DateTime? EffectiveDateFrom,
    DateTime? EffectiveDateTo,
    DateTime? PaymentReceivedDateFrom,
    DateTime? PaymentReceivedDateTo,
    DateTime? CreatedAtFrom,
    DateTime? CreatedAtTo);

public enum Status
{
    Success = 1,
    Fail = 2,
    Processing = 3,
    Pending = 4
}

public record PaginateResponse<T>(List<T> Data, int Page, int PageSize, int Total, string? OrderBy,
    string? OrderDir);


public interface IDepositWithdrawService
{
    Task<PaginateResponse<TransactionHistoryV2>> GetTransactionHistoriesV2(TransactionHistoryV2Filter filters, CancellationToken cancellationToken = default);
    Task<TransactionV2?> GetTransactionV2ByTransactionNo(string transactionNo, CancellationToken cancellationToken = default);
    Task<List<Product>> GetProducts();
    Task<List<DepositChannel>> GetDepositChannels();
    Task<List<WithdrawChannel>> GetWithdrawChannels();
}
