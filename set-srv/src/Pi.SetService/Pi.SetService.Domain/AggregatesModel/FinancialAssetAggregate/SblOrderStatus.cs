using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public enum SblOrderStatus
{
    [Description("Waiting Approval")]
    Pending,
    [Description("Approved")]
    Approved,
    [Description("Rejected")]
    Rejected
}
