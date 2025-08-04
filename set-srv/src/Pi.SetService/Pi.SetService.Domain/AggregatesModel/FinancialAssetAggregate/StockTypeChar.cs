using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public enum StockTypeChar
{
    // TODO: Need to investigate & update later after one-port integration
    [Description(" ")] None,
    [Description("H")] Unknown
}