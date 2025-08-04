namespace Pi.Financial.FundService.Application.Models.Trading;

public record CreateSubscriptionResponse
{
    public required string TransactionId { get; init; }
    public required string SaOrderReferenceNo { get; init; }
    public required string UnitHolderId { get; init; }
}
