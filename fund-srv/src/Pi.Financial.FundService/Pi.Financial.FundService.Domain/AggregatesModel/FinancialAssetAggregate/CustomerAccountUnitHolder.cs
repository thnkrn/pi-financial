namespace Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

public class CustomerAccountUnitHolder
{
    public required string AmcCode { get; init; }
    public required string UnitHolderId { get; init; }
    public required string TradingAccountNo { get; init; }
    public required UnitHolderStatus Status { get; init; }
    public required UnitHolderType UnitHolderType { get; init; }
}
