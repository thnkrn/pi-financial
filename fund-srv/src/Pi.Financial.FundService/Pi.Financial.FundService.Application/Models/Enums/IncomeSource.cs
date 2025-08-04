using System.ComponentModel;

namespace Pi.Financial.FundService.Application.Models.Enums;

public enum IncomeSource
{
    [Description("SALARY")] Salary,
    [Description("SAVINGS")] Savings,
    [Description("RETIREMENT")] Retirement,
    [Description("HERITAGE")] Heritage,
    [Description("INVESTMENT")] Investment,
    [Description("BUSINESS")] Business,
    [Description("OTHER")] Other,
}
