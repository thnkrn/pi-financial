namespace Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

public interface IAsset
{
    string FundCode { get; }
    decimal Unit { get; }
    decimal MarketPrice { get; }
    DateOnly AsOfDate { get; }
    decimal MarketValue { get; }
    decimal AvgCostPrice { get; }
}
