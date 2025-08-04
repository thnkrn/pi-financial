using System.Linq.Expressions;
using MassTransit;

namespace Pi.WalletService.IntegrationEvents.Models;

public static class DepositState
{
    public static State? Received { get; set; }
    public static State? TransactionNoGenerating { get; set; }
    public static State? DepositQrCodeGenerating { get; set; }
    public static State? DepositWaitingForPayment { get; set; }
    public static State? DepositPaymentReceived { get; set; }
    public static State? DepositPaymentNotReceived { get; set; }
    public static State? DepositPaymentSourceValidating { get; set; }
    public static State? DepositPaymentNameValidating { get; set; }
    public static State? DepositPaymentAmountValidating { get; set; }
    public static State? DepositFailedNameMismatch { get; set; }
    public static State? DepositFailedInvalidSource { get; set; }
    public static State? DepositCompleted { get; set; }
    public static State? DepositFailed { get; set; }
    public static State? DepositFailedAmountMismatch { get; set; }
    public static State? NameMismatchApproved { get; set; }
    public static State? DepositRefunding { get; set; }
    public static State? DepositRefundSucceed { get; set; }
    public static State? DepositRefundFailed { get; set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static void CleanUp()
    {
        Received = null;
        TransactionNoGenerating = null;
        DepositQrCodeGenerating = null;
        DepositWaitingForPayment = null;
        DepositPaymentReceived = null;
        DepositPaymentNotReceived = null;
        DepositPaymentSourceValidating = null;
        DepositPaymentNameValidating = null;
        DepositPaymentAmountValidating = null;
        DepositCompleted = null;
        DepositFailed = null;
        DepositFailedAmountMismatch = null;
        DepositFailedNameMismatch = null;
        DepositFailedInvalidSource = null;
        NameMismatchApproved = null;
        DepositRefunding = null;
        DepositRefundSucceed = null;
        DepositRefundFailed = null;
    }

    public static IEnumerable<State?> GetRefundAllowedStates()
    {
        return new[] { DepositFailedNameMismatch, DepositFailedAmountMismatch };
    }
}
