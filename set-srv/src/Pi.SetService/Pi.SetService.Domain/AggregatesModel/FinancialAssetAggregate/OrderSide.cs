using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public enum OrderSide
{
    [Description("Buy")] Buy,
    [Description("Sell")] Sell
}
