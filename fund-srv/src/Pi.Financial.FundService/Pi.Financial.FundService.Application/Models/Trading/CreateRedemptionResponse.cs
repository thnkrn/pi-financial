namespace Pi.Financial.FundService.Application.Models.Trading;

public class CreateRedemptionResponse
{
    public required string TransactionId { get; init; }
    public required string SaOrderReferenceNo { get; init; }
    public required DateOnly SettlementDate { get; init; }
}
