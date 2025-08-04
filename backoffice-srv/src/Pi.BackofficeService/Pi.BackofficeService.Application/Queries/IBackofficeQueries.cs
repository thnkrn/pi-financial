using Pi.BackofficeService.Application.Models.Commons;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;
using DepositChannel = Pi.BackofficeService.Application.Models.DepositChannel;
using WithdrawChannel = Pi.BackofficeService.Application.Models.WithdrawChannel;

namespace Pi.BackofficeService.Application.Queries;

public record TransactionResult<T>()
{
    public required T Transaction { get; init; }
    public required ResponseCode? ResponseCode { get; init; }
}

public record TransactionDetailResult<T>()
{
    public required T Transaction { get; init; }
    public required ResponseCodeDetail? ResponseCodeDetail { get; init; }
}

public record TransactionFilter(
    Channel? Channel,
    TransactionType? TransactionType,
    Product? Product,
    ProductType? ProductType,
    Guid? ResponseCodeId,
    string? BankCode,
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

public record TransferCashFilter(
    string? Status,
    string? State,
    string? TransactionNo,
    string? TransferFromAccountCode,
    string? TransferToAccountCode,
    Product? TransferFromExchangeMarket,
    Product? TransferToExchangeMarket,
    DateTime? OtpConfirmedDateFrom,
    DateTime? OtpConfirmedDateTo,
    DateTime? CreatedAtFrom,
    DateTime? CreatedAtTo);

public record ResponseCodeDetail(Guid Id, Machine Machine, string State, string? Suggestion, string Description, List<ResponseCodeAction> Actions);

public interface IBackofficeQueries
{
    // Deposit Withdraw
    Task<TransactionDetailResult<TransactionV2>?> GetTransactionV2ByTransactionNo(string transactionNo, CancellationToken cancellationToken = default);
    Task<PaginateResult<TransactionResult<TransactionHistoryV2>>> GetTransactionsV2Paginate(int? pageNum, int? pageSize,
        string? orderBy, string? orderDir, TransactionFilter? filters, CancellationToken cancellationToken = default);

    // Transfer cash
    Task<TransactionDetailResult<TransferCash>?> GetTransferCashByTransactionNo(string transactionNo,
        CancellationToken cancellationToken = default);
    Task<PaginateResult<TransactionResult<TransferCash>>?> GetTransferCashPaginate(int? pageNum, int? pageSize,
        string? orderBy, string? orderDir, TransferCashFilter? filters, CancellationToken cancellationToken = default);

    Task<List<Product>> GetProducts(ProductType? productType);
    Task<List<DepositChannel>> GetDepositChannels();
    Task<List<WithdrawChannel>> GetWithdrawChannels();
    Task<List<Bank>> GetBanks(string? channel);
    Task<List<ResponseCode>> GetResponseCodes(ResponseCodeFilter filter);
    Task<List<ResponseCodeAction>> GetResponseCodeAction(Guid id);
}
