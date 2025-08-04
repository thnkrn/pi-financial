using Pi.TfexService.Domain.SeedWork;

namespace Pi.TfexService.Domain.Models.ActivitiesLog;

public class ActivitiesLog(
    Guid id,
    string userId,
    string customerCode,
    string accountCode,
    RequestType requestType,
    string? requestBody,
    string? orderNo,
    string? responseBody,
    DateTime? requestedAt,
    DateTime? completedAt,
    bool isSuccess,
    string? failedReason,
    string? symbol,
    string? side,
    string? priceType,
    decimal? price,
    int? qty,
    string? rejectCode,
    string? rejectReason)
    : BaseEntity
{
    public Guid Id { get; private set; } = id;
    public string UserId { get; private set; } = userId;
    public string CustomerCode { get; private set; } = customerCode;
    public string AccountCode { get; private set; } = accountCode;
    public RequestType RequestType { get; private set; } = requestType;
    public string? RequestBody { get; private set; } = requestBody;
    public string? OrderNo { get; private set; } = orderNo;
    public string? ResponseBody { get; private set; } = responseBody;
    public DateTime? RequestedAt { get; private set; } = requestedAt;
    public DateTime? CompletedAt { get; private set; } = completedAt;
    public bool IsSuccess { get; private set; } = isSuccess;
    public string? FailedReason { get; private set; } = failedReason;
    public string? Symbol { get; private set; } = symbol;
    public string? Side { get; private set; } = side;
    public string? PriceType { get; private set; } = priceType;
    public decimal? Price { get; private set; } = price;
    public int? Qty { get; private set; } = qty;
    public string? RejectCode { get; private set; } = rejectCode;
    public string? RejectReason { get; private set; } = rejectReason;

}