using System.ComponentModel;

namespace Pi.PortfolioService.Models;

public enum PortfolioAccountSuffix
{
    [Description("1")]
    Cash,
    [Description("6")]
    CreditBalance,
    [Description("8")]
    CashBalance,
}