using System.ComponentModel;

namespace Pi.Financial.FundService.Application.Models.Enums;

public enum Title
{
    [Description("MR")] Mr,
    [Description("MRS")] Mrs,
    [Description("MISS")] Miss,
    [Description("OTHER")] Other,
}
