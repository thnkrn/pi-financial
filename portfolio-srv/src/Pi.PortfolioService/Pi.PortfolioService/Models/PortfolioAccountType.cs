using System.ComponentModel;

namespace Pi.PortfolioService.Models;

public enum PortfolioAccountType
{
    [Description("Structure Notes")]
    Offshore = 1,
    [Description("Mutual Fund")]
    MutualFund,
    [Description("Cash")]
    Cash,
    [Description("Cash Balance")]
    CashBalance,
    [Description("Credit Balance/SBL")]
    CreditBalance,
    [Description("Global Equity")]
    GlobalEquities,
    [Description("Derivative")]
    Derivative,
    [Description("Bond")]
    Bond,
    [Description("Bond Cash Balance")]
    BondCashBalance
}
