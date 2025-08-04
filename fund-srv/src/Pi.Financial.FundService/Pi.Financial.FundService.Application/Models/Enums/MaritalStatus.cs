using System.ComponentModel;

namespace Pi.Financial.FundService.Application.Models.Enums;

public enum MaritalStatus
{
    [Description("Single")] Single = 1,
    [Description("Married")] Married = 2,
    [Description("Widowed")] Widowed = 3,
    [Description("Divorced")] Divorced = 4,
}
