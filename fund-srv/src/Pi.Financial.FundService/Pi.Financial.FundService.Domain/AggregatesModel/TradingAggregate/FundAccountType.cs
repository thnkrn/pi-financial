using System.ComponentModel;

namespace Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

public enum FundAccountType
{
    [Description("SEG")]
    SEG,
    [Description("OMN")]
    OMN,
    [Description("SEG_NT")]
    SEG_NT,
    [Description("SEG_T")]
    SEG_T,
    [Description("OMN_NT")]
    OMN_NT,
    [Description("OMN_T")]
    OMN_T
}
