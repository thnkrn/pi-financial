using System.ComponentModel;

namespace Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

public enum FundOrderStatus
{
    [Description("Submitted")]
    Submitted,
    [Description("Approved")]
    Approved,
    [Description("Cancelled")]
    Cancelled,
    [Description("Rejected")]
    Rejected,
    [Description("Allotted")]
    Allotted,
    [Description("Expired")]
    Expired,
    [Description("Waiting")]
    Waiting
}
