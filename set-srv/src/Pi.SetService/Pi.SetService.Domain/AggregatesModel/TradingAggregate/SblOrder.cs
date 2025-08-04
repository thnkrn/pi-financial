using Pi.Common.SeedWork;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Domain.AggregatesModel.TradingAggregate;

public class SblOrder : IAggregateRoot, IAuditableEntity
{
    // Constructor required for Entity framework
    public SblOrder(Guid id, Guid tradingAccountId, string tradingAccountNo, string customerCode, ulong orderId,
        string symbol, SblOrderStatus status, int volume, SblOrderType type, string? rejectedReason, Guid? reviewerId)
    {
        Id = id;
        TradingAccountId = tradingAccountId;
        TradingAccountNo = tradingAccountNo;
        CustomerCode = customerCode;
        OrderId = orderId;
        Symbol = symbol;
        Type = type;
        Volume = volume;
        Status = status;
        RejectedReason = rejectedReason;
        ReviewerId = reviewerId;
    }

    public SblOrder(Guid id, TradingAccount tradingAccount, ulong orderId, string symbol, int volume, SblOrderType type)
    {
        Id = id;
        TradingAccountId = tradingAccount.Id;
        TradingAccountNo = tradingAccount.TradingAccountNo;
        CustomerCode = tradingAccount.CustomerCode;
        OrderId = orderId;
        Symbol = symbol;
        Status = SblOrderStatus.Pending;
        Volume = volume;
        Type = type;
    }

    public Guid Id { get; init; }
    public Guid TradingAccountId { get; init; }
    public string TradingAccountNo { get; init; }
    public string CustomerCode { get; init; }
    public ulong OrderId { get; private set; }
    public string Symbol { get; init; }
    public SblOrderType Type { get; init; }
    public int Volume { get; private set; }
    public SblOrderStatus Status { get; private set; }
    public string? RejectedReason { get; private set; }
    public Guid? ReviewerId { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateOnly CreatedAtDate { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public void UpdateOrderId(ulong orderId)
    {
        OrderId = orderId;
    }

    public void Approve(Guid reviewerId)
    {
        ReviewerId = reviewerId;
        Status = SblOrderStatus.Approved;
    }

    public void Reject(Guid reviewerId, string? reason)
    {
        ReviewerId = reviewerId;
        Status = SblOrderStatus.Rejected;
        RejectedReason = reason;
    }
}
