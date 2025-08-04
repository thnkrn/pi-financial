using System.Linq.Expressions;
using MassTransit;

namespace Pi.WalletService.IntegrationEvents.Models;

public static class WithdrawState
{
    public static State? Received { get; set; }
    public static State? TransactionNoGenerating { get; set; }
    public static State? WithdrawalInitiating { get; set; }
    public static State? RequestingOtpValidation { get; set; }
    public static State? RequestingOtpValidationFailed { get; set; }
    public static State? OtpValidationNotReceived { get; set; }
    public static State? WaitingForOtpValidation { get; set; }
    public static State? WaitingForConfirmation { get; set; }
    public static State? WithdrawalProcessing { get; set; }
    public static State? WithdrawalFailed { get; set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static void CleanUp()
    {
        Received = null;
        TransactionNoGenerating = null;
        WithdrawalInitiating = null;
        RequestingOtpValidation = null;
        RequestingOtpValidationFailed = null;
        OtpValidationNotReceived = null;
        WaitingForOtpValidation = null;
        WaitingForConfirmation = null;
        WithdrawalProcessing = null;
        WithdrawalFailed = null;
    }
}
