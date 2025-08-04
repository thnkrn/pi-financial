using System.ComponentModel;

namespace Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

public enum Channel
{
    [Description("MOB")]
    MOB,
    [Description("ONL")]
    ONL,
    [Description("MKT")]
    MKT
}
