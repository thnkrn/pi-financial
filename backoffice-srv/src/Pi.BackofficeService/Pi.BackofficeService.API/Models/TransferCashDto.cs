using Pi.BackofficeService.API.Factories;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;
using ProductDto = Pi.BackofficeService.Application.Models.Product;

namespace Pi.BackofficeService.API.Models;

public record TransferCashFilterRequest(
    string? Status,
    string? State,
    string? TransactionNo,
    string? TransferFromAccountCode,
    string? TransferToAccountCode,
    Product? TransferFromExchangeMarket,
    Product? TransferToExchangeMarket,
    DateOnly? OtpConfirmedDateFrom,
    DateOnly? OtpConfirmedDateTo,
    DateOnly? CreatedAtFrom,
    DateOnly? CreatedAtTo);

public record TransferCashPaginateRequest(
    string? OrderBy,
    string? OrderDir,
    TransferCashFilterRequest? Filters,
    int? Page,
    int? PageSize);

public record TransferCashBase
{
    public TransferCashBase(TransferCash transaction)
    {
        State = transaction.State;
        Status = transaction.Status;
        TransactionNo = transaction.TransactionNo;
        CustomerName = transaction.CustomerName;
        TransferFromAccountCode = transaction.TransferFromAccountCode;
        TransferFromExchangeMarket = DtoFactory.NewNameAliasResponse((ProductDto)transaction.TransferFromExchangeMarket!);
        TransferToAccountCode = transaction.TransferToAccountCode;
        TransferToExchangeMarket = DtoFactory.NewNameAliasResponse((ProductDto)transaction.TransferToExchangeMarket!);
        Amount = transaction.Amount;
        FailedReason = transaction.FailedReason;
        OtpConfirmedDateTime = transaction.OtpConfirmedDateTime;
        CreatedAt = transaction.CreatedAt;
    }
    public string? State { get; set; }
    public string? TransactionNo { get; set; }
    public string? Status { get; set; }
    public string? CustomerName { get; set; }
    public string? TransferFromAccountCode { get; set; }
    public string? TransferToAccountCode { get; set; }
    public NameAliasResponse? TransferFromExchangeMarket { get; set; }
    public NameAliasResponse? TransferToExchangeMarket { get; set; }
    public decimal? Amount { get; set; }
    public string? FailedReason { get; set; }
    public DateTime? OtpConfirmedDateTime { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public record TransferCashDetailResponse : TransferCashBase
{
    public TransferCashDetailResponse(TransactionDetailResult<TransferCash> transactionResult) : base(transactionResult.Transaction)
    {
        ResponseCode = transactionResult.ResponseCodeDetail != null
            ? DtoFactory.NewResponseCodeDetailResponse(transactionResult.ResponseCodeDetail)
            : null;
    }
    public ResponseCodeDetailResponse? ResponseCode { get; init; }
}

public record TransferCashResponse : TransferCashBase
{
    public TransferCashResponse(TransactionResult<TransferCash> transactionResult) : base(transactionResult.Transaction)
    {
        ResponseCode = transactionResult.ResponseCode != null
            ? DtoFactory.NewResponseCodeResponse(transactionResult.ResponseCode)
            : null;
    }
    public ResponseCodeResponse? ResponseCode { get; init; }
}