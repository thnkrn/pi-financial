using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;

namespace Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

public record CustomerAccountDetail
{
    public required string IcLicense { get; init; }
    public required List<CustomerAccountUnitHolder> CustomerAccountUnitHolders { get; init; }
    public InvestorClass? InvestorClass { get; init; }
    public InvestorType? InvestorType { get; init; }
    public string? JuristicNumber { get; init; }
}
