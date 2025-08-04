using Pi.Financial.FundService.Application.Models.Metric;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Factories;

public static class MetricFactory
{
    public static KeyValuePair<string, object?>[] NewFundOrderTag(FundOrderState fundOrderState)
    {
        return new KeyValuePair<string, object?>[]
        {
            new("order_side", fundOrderState.OrderSide.ToString()),
            new("order_type", fundOrderState.OrderType.ToString()),
            new("redemption_type", fundOrderState.RedemptionType?.ToString())
        };
    }

    public static KeyValuePair<string, object?>[] NewFundOrderTag(MetricTags metricTags)
    {
        return new KeyValuePair<string, object?>[]
        {
            new("order_side", metricTags.OrderSide.ToString()),
            new("order_type", metricTags.OrderType.ToString()),
            new("redemption_type", metricTags.RedemptionType.ToString()),
            new("error_code", metricTags.ErrorCode.ToString())
        };
    }
}
