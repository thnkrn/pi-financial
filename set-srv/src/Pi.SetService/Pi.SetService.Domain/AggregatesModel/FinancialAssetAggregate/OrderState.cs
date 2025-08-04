using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public enum OrderState
{
    [Description("Pending")] Pending,
    [Description("Pending")] S,
    [Description("Queuing")] SX,
    [Description("Open")] O,
    [Description("Queuing")] C,
    [Description("Rejected")] R,
    [Description("Approved")] A,
    [Description("Matched")] M,
    [Description("M-Partially")] MP,
    [Description("Expired")] E,

    // NOTE: In Sirius code is TBC for description
    [Description("Pending")] PO,
    [Description("Pending")] PC,
    [Description("Pending")] POA,
    [Description("Pending")] PX,
    [Description("Pending")] PXA,
    [Description("Pending")] PXC,
    [Description("Pending")] m,
    [Description("Pending")] OC,

    // NOTE: In Sirius code is TBC for description
    [Description("Open")] OA, // Approved
    [Description("Disapproved")] D,
    [Description("Partial Match")] MA, // Approved
    [Description("Partial Match")] MC, // Changed
    [Description("Partial Match")] MAC, // Approved & Changed
    [Description("Partial Match")] MD, // Matched (PTD)
    [Description("Partial Match")] MDC, // Matched (PTD)
    [Description("Partial Match")] MPC,
    [Description("Changer")] OAC, // Approved & Changed
    [Description("Cancelled")] X,
    [Description("Cancelled")] XA, // Approved
    [Description("Cancelled")] XC, // Changed
    [Description("Cancelled")] XAC, // Approved & Changed
    [Description("Review")] UO,
    [Description("Review")] UX, // Canceled
    [Description("DS")] DS, // Pending Approve (PTD Entry)
    [Description("DC")] DC, // Receive (PTD Entry)
    [Description("SD")] SD, // Disapproved (PTD)
    [Description("CD")] CD, // Disapproved (PTD)
    [Description("XS")] XS, // Cancel (PTD)
    [Description("XB")] XB, // Additional status
    [Description("Pending(OF)")] OF
}
