namespace Pi.Financial.FundService.Domain.Events;

public record SubscriptionOrderPlaced
{
    public required string CustCode { get; init; }
    public required string TradingAccountNo { get; init; }
    public required string FundCode { get; init; }
    public required string UnitHolderId { get; init; }
    public required string TransactionId { get; init; }
    public required string SaOrderReferenceNo { get; init; }
}
