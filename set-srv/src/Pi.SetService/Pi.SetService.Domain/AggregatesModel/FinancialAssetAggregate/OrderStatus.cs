using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public enum OrderStatus
{
    [Description("Pending")] Pending,
    [Description("Pending({0})")] PendingEx,
    [Description("Pending(Trigger)")] PendingTg,
    [Description("Queuing")] Queuing,
    [Description("Queuing(%s)")] QueuingEx,
    [Description("Matched")] Matched,
    [Description("Matching")] Matching,
    [Description("PartialMatch")] PartialMatch,
    [Description("MatchedEx")] MatchedEx,
    [Description("Cancelled")] Cancelled,
    [Description("CancelledEx")] CancelledEx,
    [Description("Rejected")] Rejected,
    [Description("Unknown({0})")] Unknown
}
