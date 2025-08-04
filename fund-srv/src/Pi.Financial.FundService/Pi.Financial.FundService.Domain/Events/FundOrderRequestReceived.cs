using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Domain.Events;

public record FundOrderRequestReceived
{
    public required Guid CorrelationId { get; set; }
    public required Guid TradingAccountId { get; set; }
    public required string FundCode { get; set; }
    public required DateOnly EffectiveDate { get; set; }
    public required Channel Channel { get; set; }
    public required string TradingAccountNo { get; set; }
    public required string CustomerCode { get; set; }
}

public record SubscriptionFundRequestReceived : FundOrderRequestReceived
{
    public required decimal Amount { get; set; }
    public required PaymentType PaymentType { get; set; }
    public required string? UnitHolderId { get; init; }
    public required string BankAccount { get; set; }
    public required string BankCode { get; set; }
    public bool Recurring { get; set; }
}

public record RedemptionFundRequestReceived : FundOrderRequestReceived
{
    public required string BankAccount { get; set; }
    public required string BankCode { get; set; }
    public required decimal? Amount { get; set; }
    public required decimal? Unit { get; set; }
    public required RedemptionType RedemptionType { get; init; }
    public required string UnitHolderId { get; init; }
    public required bool? SellAllFlag { get; init; }
}

public record SwitchingFundRequestReceived : FundOrderRequestReceived
{
    public required decimal? Amount { get; set; }
    public required decimal? Unit { get; set; }
    public required string CounterFundCode { get; set; }
    public required RedemptionType RedemptionType { get; init; }
    public required string UnitHolderId { get; init; }
    public required bool? SellAllFlag { get; init; }
}
