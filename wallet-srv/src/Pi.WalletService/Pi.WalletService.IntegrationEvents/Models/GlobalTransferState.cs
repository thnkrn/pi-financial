using System.Linq.Expressions;
using MassTransit;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.IntegrationEvents.Models;

public static class GlobalTransferState
{
    public static State? Initial { get; set; }
    public static State? Received { get; set; }
    public static State? PaymentReceivedDataPreparing { get; set; }
    public static State? TransferRequestFailed { get; set; }
    public static State? GlobalTransferPaymentValidating { get; set; }
    public static State? FxInitiating { get; set; }
    public static State? FxValidating { get; set; }
    public static State? FxConfirming { get; set; }
    public static State? FxFailed { get; set; }
    public static State? FxRateCompareFailed { get; set; }
    public static State? FxQueryTransaction { get; set; }
    public static State? TransferRequestValidating { get; set; }
    public static State? DepositFxTransferring { get; set; }
    public static State? WithdrawFxTransferring { get; set; }
    public static State? GlobalTransferCompleted { get; set; }
    public static State? GlobalTransferFailed { get; set; }
    public static State? FxTransferFailed { get; set; }
    public static State? FxTransferInsufficientBalance { get; set; }
    public static State? RevertTransferSuccess { get; set; }
    public static State? ManualAllocationInprogress { get; set; }
    public static State? ManualAllocationFailed { get; set; }
    public static State? Refunding { get; set; }
    public static State? RefundSucceed { get; set; }
    public static State? RefundFailed { get; set; }
    public static State? GlobalDepositPaymentNotReceived { get; set; }
    public static State? GlobalWithdrawOtpValidationNotReceived { get; set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static void CleanUp()
    {
        Initial = null;
        PaymentReceivedDataPreparing = null;
        Received = null;
        TransferRequestValidating = null;
        TransferRequestFailed = null;
        GlobalTransferPaymentValidating = null;
        FxInitiating = null;
        FxValidating = null;
        FxConfirming = null;
        FxFailed = null;
        FxRateCompareFailed = null;
        FxQueryTransaction = null;
        DepositFxTransferring = null;
        WithdrawFxTransferring = null;
        GlobalTransferCompleted = null;
        GlobalTransferFailed = null;
        FxTransferFailed = null;
        FxTransferInsufficientBalance = null;
        RevertTransferSuccess = null;
        ManualAllocationInprogress = null;
        ManualAllocationFailed = null;
        Refunding = null;
        RefundSucceed = null;
        RefundFailed = null;
        GlobalDepositPaymentNotReceived = null;
        GlobalWithdrawOtpValidationNotReceived = null;
    }

    public static IEnumerable<State?> GetRefundAllowedStates()
    {
        return new[] { FxFailed, FxRateCompareFailed };
    }

    public static IEnumerable<(State?, TransactionType?)> GetManualAllocationAllowedStates()
    {
        return new (State?, TransactionType?)[]
        {
            (FxTransferInsufficientBalance, TransactionType.Deposit),
            (FxTransferFailed, TransactionType.Deposit),
            (ManualAllocationFailed, null)
        };
    }
}
