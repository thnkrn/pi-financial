namespace Pi.Financial.FundService.Application.Models.Metric;

public interface IMetrics
{
    IEnumerable<string> GetMetricsCounter();
    IEnumerable<string> GetMetricsUpDownCounter();
}

public class Metrics : IMetrics
{
    #region fund order 

    public const string FundOrderReceived = "fund_order.received";
    public const string FundOrderCreated = "fund_order.created";
    public const string FundOrderPlacedSuccess = "fund_order.placed_success";
    public const string FundOrderPlacedFailed = "fund_order.placed_failed";
    public const string FundOrderPlacedUnit = "fund_order.placed_unit";
    public const string FundOrderPlacedAmount = "fund_order.placed_amount";
    public const string FundOrderError = "fund_order.error";

    #endregion

    public IEnumerable<string> GetMetricsCounter()
    {
        return new[]
        {
            FundOrderReceived, FundOrderCreated, FundOrderPlacedSuccess, FundOrderPlacedFailed, FundOrderPlacedUnit,
            FundOrderPlacedAmount, FundOrderError
        };
    }

    public IEnumerable<string> GetMetricsUpDownCounter()
    {
        return Array.Empty<string>();
    }
}
