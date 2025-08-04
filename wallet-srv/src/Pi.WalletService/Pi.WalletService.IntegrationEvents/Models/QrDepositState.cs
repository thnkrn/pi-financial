using System.Linq.Expressions;
using MassTransit;

namespace Pi.WalletService.IntegrationEvents.Models;

public static class QrDepositState
{
    public static State? Initial { get; set; }
    public static State? Received { get; set; }
    public static State? QrCodeGenerating { get; set; }
    public static State? WaitingForPayment { get; set; }
    public static State? DepositEntrypointUpdating { get; set; }
    public static State? PaymentSourceValidating { get; set; }
    public static State? PaymentNameValidating { get; set; }
    public static State? PaymentAmountValidating { get; set; }
    public static State? DepositFailedNameMismatch { get; set; }
    public static State? DepositFailedInvalidSource { get; set; }
    public static State? DepositFailedAmountMismatch { get; set; }
    public static State? NameMismatchApproved { get; set; }
    public static State? QrDepositCompleted { get; set; }
    public static State? QrDepositFailed { get; set; }
    public static State? Refunding { get; set; }
    public static State? RefundSucceed { get; set; }
    public static State? RefundFailed { get; set; }
    public static State? PaymentNotReceived { get; set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static void CleanUp()
    {
        Initial = null;
        Received = null;
        QrCodeGenerating = null;
        WaitingForPayment = null;
        DepositEntrypointUpdating = null;
        PaymentSourceValidating = null;
        PaymentNameValidating = null;
        PaymentAmountValidating = null;
        DepositFailedAmountMismatch = null;
        DepositFailedNameMismatch = null;
        DepositFailedInvalidSource = null;
        NameMismatchApproved = null;
        QrDepositCompleted = null;
        QrDepositFailed = null;
        Refunding = null;
        RefundSucceed = null;
        RefundFailed = null;
        PaymentNotReceived = null;
    }

    public static IEnumerable<State?> GetRefundAllowedStates()
    {
        return new[] { DepositFailedNameMismatch, DepositFailedAmountMismatch };
    }
}
