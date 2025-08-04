using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.API.Models;

// TODO: Remove this class and create new DTO when migrated FE from Sirius is done
public record SiriusFundOrderResponse
{
    public required decimal Unit { get; init; }
    public required decimal Amount { get; init; }
    public required string PaymentMethod { get; init; }
    public required string BankAccount { get; init; }
    public required string? Edd { get; init; }
    public required string? SwitchIn { get; init; }
    public required string? SwitchTo { get; init; }
    public required string BankCode { get; init; }
    public required string BankShortName { get; init; }
    public required string OrderId { get; init; }
    public required string Account { get; init; }
    public required string FundCode { get; init; }
    public required string OrderType { get; init; }
    public required SiriusFundOrderStatus Status { get; init; }
    public required decimal? Nav { get; init; }
    public required string TransactionLastUpdated { get; init; }
    public required string EffectiveDate { get; init; }
    public required string TransactionDateTime { get; init; }
    public string? SettlementDate { get; init; }
    public string? OrderNo { get; init; }
}

public record BrokerOrder
{
    public required string BrokerOrderId { get; init; }
}

public record FundOrderPlaced : BrokerOrder
{
    public required string OrderNo { get; init; }
    public required string UnitHolderId { get; init; }
    public string? SettlementDate { get; init; }
}

public enum SiriusFundOrderStatus
{
    [Description("Processing")] Processing,
    [Description("Matched")] Matched,
    [Description("Pending")] Pending,
    [Description("Cancelled")] Cancelled,
    [Description("Rejected")] Rejected
}

public record SwitchInfoRequest
{
    [Required]
    public required string TradingAccountNo { get; init; }
    [Required]
    public required string Symbol { get; init; }
    [Required]
    public required string TargetSymbol { get; init; }
}

public record DeleteOrderRequest : BrokerOrder
{
    [Required]
    public required OrderSide OrderSide { get; init; }
    [Required]
    public required string TradingAccountNo { get; init; }
    public bool? Force { get; init; }
}

public record SiriusFundOrderHistoryResponse
{
    public required decimal Unit { get; init; }
    public required decimal Amount { get; init; }
    public required string FundCode { get; init; }
    public required string PaymentMethod { get; init; }
    public required string BankCode { get; init; }
    public required string BankShortName { get; init; }
    public required string BankAccount { get; init; }
    public required string OrderId { get; init; }
    public required string Account { get; init; }
    public required string OrderType { get; init; }
    public required SiriusFundOrderStatus Status { get; init; }
    public required decimal? Nav { get; init; }
    public required string EffectiveDate { get; init; }
    public required string TransactionDateTime { get; init; }
    public required string UnitHolderId { get; init; }
    public required Channel Channel { get; init; }
    public required FundAccountType? AccountType { get; init; }
    public string? SettlementDate { get; init; }
    public string? OrderNo { get; init; }
    public string? RejectReason { get; init; }
    public string? TransactionLastUpdated { get; init; }
    public string? PaymentStatus { get; set; }
}
