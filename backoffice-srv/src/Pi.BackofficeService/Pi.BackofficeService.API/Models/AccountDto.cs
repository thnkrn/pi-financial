using System.ComponentModel;

namespace Pi.BackofficeService.API.Models;

public enum ProductResponse
{
    [Description("SET(Cash)")] Cash,
    [Description("SET(Cash Balance)")] CashBalance,
    [Description("SET(Margin)")] Margin,
    Crypto,
    [Description("TFEX")] TFEX,
    [Description("Global Equity")] GlobalEquity,
    [Description("Mutual Fund")] Funds
}
