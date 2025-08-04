using System.ComponentModel;

namespace Pi.BackofficeService.Application.Models;

public enum ReportType
{
    [Description("Deposit/Withdraw Reconcile Report (ALL)")] AllDWReconcile,
    [Description("Pending Transaction Report")] PendingTransaction,
    [Description("Bloomberg Equity Closeprice")] BloombergEOD,
    [Description("Velexa Transactions")] VelexaEODTransaction,
    [Description("Velexa Trades Vat")] VelexaEODTradeVAT,
    [Description("Velexa Account Summary")] VelexaEODAccountSummary,
    [Description("Velexa Trades")] VelexaEODTrade,

    // Pi App D/W Daily
    [Description("Pi App D/W Daily Report")] PiAppDepositWithdrawDailyReport,
    [Description("Pi App Global Reconcile Report")] PiAppGlobalReconcileReport,
    [Description("Bill Payment Reconcile Report")] BillPaymentReconcileReport
}
