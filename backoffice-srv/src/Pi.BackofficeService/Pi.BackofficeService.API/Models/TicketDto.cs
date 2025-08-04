using System.ComponentModel;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Models;

public record TicketRequest(
    string TransactionNo,
    TransactionType TransactionType,
    Method Method,
    string? Remark,
    string? Payload);

public record CheckTicketRequest(Method Method, string? Remark);

public record TicketResponse(
    Guid CorrelationId,
    string TicketNo,
    Guid? TransactionId,
    string TransactionNo,
    TransactionType TransactionType,
    string? CustomerName,
    string? CustomerCode,
    Status? Status,
    Method? MakerAction,
    DateTime? RequestedAt,
    Guid? MakerId,
    string? MakerRemark,
    Method? CheckerAction,
    DateTime? CheckedAt,
    Guid? CheckerId,
    string? CheckerRemark,
    ResponseCodeResponse? ResponseCode);

public record TicketDetailResponse(
    Guid CorrelationId,
    string TicketNo,
    Guid? TransactionId,
    string TransactionNo,
    TransactionType TransactionType,
    string? CustomerName,
    string? CustomerCode,
    Status? Status,
    NameAliasResponse<TicketAction>? MakerAction,
    DateTime? RequestedAt,
    UserResponse? Maker,
    string? MakerRemark,
    NameAliasResponse<TicketAction>? CheckerAction,
    DateTime? CheckedAt,
    UserResponse? Checker,
    string? CheckerRemark,
    ResponseCodeResponse? ResponseCode,
    DateTime CreatedAt);

public enum TicketAction
{
    [Description("Approve to Profile")] Approve,
    [Description("Refund to Customer")] Refund,
    [Description("CCY Allocation Transfer")] CcyAllocationTransfer,
    [Description("Retry in SBA")] RetrySbaDeposit,
    [Description("Retry in SBA")] RetrySbaWithdraw,
    [Description("Retry in Settrade")] RetrySetTradeDeposit,
    [Description("Retry in SetTrade")] RetrySetTradeWithdraw,
    [Description("Retry in KKP")] RetryKkpWithdraw,
    [Description("SBA Confirm")] SbaConfirm,
    [Description("SBA ATS Confirm")] SbaDepositAtsCallbackConfirm,
    [Description("KKP Deposit Confirm")] DepositKkpConfirm,
    [Description("Change status to pending")] ChangeStatusToPending,
    [Description("Change status to success")] ChangeStatusToSuccess,
    [Description("Change status to fail")] ChangeStatusToFail,
    [Description("Change Settrade status to pending")] ChangeSetTradeStatusToPending,
    [Description("Change Settrade status to success")] ChangeSetTradeStatusToSuccess,
    [Description("Change Settrade status to fail")] ChangeSetTradeStatusToFail,
    [Description("Edit Account No and Approve")] UpdateBillPaymentReference
}

public class TicketPaginateQuery : PaginateQuery
{
    public Guid? ResponseCodeId { get; set; }
    public string? CustomerCode { get; set; }
    public Status? Status { get; set; }
}

public record CheckTicketResponse
{
    public required bool IsSuccess { get; init; }
    public required string? ErrorMsg { get; init; }
}