using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public enum OrderAction
{
    [Description("Cover")] Cover,
    [Description("Short")] Short,
    [Description("Buy")] Buy,
    [Description("Sell")] Sell,
    [Description("Borrow")] Borrow,
    [Description("Return")] Return
}
