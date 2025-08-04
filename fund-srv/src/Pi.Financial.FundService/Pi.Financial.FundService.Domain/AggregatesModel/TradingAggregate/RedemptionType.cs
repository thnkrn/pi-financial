using System.ComponentModel;

namespace Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

public enum RedemptionType
{
    [Description("AMT")]
    Amount,
    [Description("UNIT")]
    Unit
}
