namespace Pi.Financial.FundService.Application.Options;

public class FundTradingOptions
{
    public const string Options = "FundTrading";

    public required string SaCode { get; init; }
    public decimal MinBuyAmountLimit { get; init; } = 500000;
    public required string SegAccountSuffix { get; init; }
    public required string[] SegTaxTypes { get; init; }
    public required string[] SubscriptionAvoidTaxTypes { get; init; }
}
