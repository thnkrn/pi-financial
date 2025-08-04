using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Models.Trading;

public record CreateSubscriptionRequest
{
    public required string SaOrderReferenceNo { get; init; }
    public required string SaCode { get; init; }
    public required DateTime TransactionDateTime { get; init; }
    public required string AccountId { get; init; }
    public required string UnitholderId { get; init; }
    public required bool OverrideRiskProfile { get; init; }
    public required bool OverrideFxRisk { get; init; }
    public required string FundCode { get; init; }
    public required decimal Amount { get; init; }
    public required DateOnly EffectiveDate { get; init; }
    public required string? BankCode { get; init; }
    public required string? BankAccount { get; init; }
    public required Channel Channel { get; init; }
    public required string SaleLicense { get; init; }
    public required bool ForceEntry { get; init; }
    public required PaymentType PaymentType { get; init; }
}
