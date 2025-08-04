using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Models.Metric;

public class MetricTags
{
    public required OrderSide OrderSide;
    public required FundOrderType OrderType;
    public RedemptionType? RedemptionType;
    public FundOrderErrorCode? ErrorCode;
}
