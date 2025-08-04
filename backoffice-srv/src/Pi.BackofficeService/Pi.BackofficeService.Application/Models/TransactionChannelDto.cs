using System.ComponentModel;

namespace Pi.BackofficeService.Application.Models;

public enum DepositChannel
{
    [Description("QR")] QR,
    [Description("ODD")] ODD,
    [Description("ATS")] AtsBatch,
    [Description("SetTrade E-Payment")] SetTrade,
    [Description("Bill Payment")] BillPayment,
    // [Description("ODD")] Odd,
    // [Description("Cross Bank Bill Payment")] CrossBankBillPayment
}

public enum WithdrawChannel
{
    [Description("Online Transfer")] OnlineTransfer,
    [Description("ATS")] AtsBatch
}

public enum TransactionChannel
{
    [Description("QR")] QR,
    [Description("ODD")] ODD,
    [Description("ATS")] AtsBatch,
    [Description("Bill Payment")] BillPayment,
    [Description("SetTrade E-Payment")] SetTrade,
    [Description("Online Transfer")] OnlineTransfer,
}
