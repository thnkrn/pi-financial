namespace Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

public interface IUpnl
{
    decimal MarketValue { get; }
    decimal CostValue { get; }
    decimal UPNL { get; }
    decimal UPNLPercentage { get; }
}
