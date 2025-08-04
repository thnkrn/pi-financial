using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public enum TradingAccountType
{
    [Description("Cash")]
    Cash,
    [Description("Cash Margin Account")]
    CashMargin,
    [Description("Maintenance Margin")]
    MaintenanceMargin,
    [Description("Credit Balance")]
    CreditBalance,
    [Description("Cash Balance")]
    CashBalance,
    [Description("Internet")]
    Internet
}
