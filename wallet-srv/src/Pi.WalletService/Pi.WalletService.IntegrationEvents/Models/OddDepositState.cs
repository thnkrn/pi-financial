using System.Linq.Expressions;
using MassTransit;

namespace Pi.WalletService.IntegrationEvents.Models;

public static class OddDepositState
{
    public static State? Initial { get; set; }
    public static State? Received { get; set; }
    public static State? RequestingOtpValidation { get; set; }
    public static State? RequestingOtpValidationFailed { get; set; }
    public static State? WaitingForOtpValidation { get; set; }
    public static State? OddDepositProcessing { get; set; }
    public static State? OddDepositCompleted { get; set; }
    public static State? OddDepositFailed { get; set; }
    public static State? OtpValidationNotReceived { get; set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static void CleanUp()
    {
        Initial = null;
        Received = null;
        RequestingOtpValidation = null;
        RequestingOtpValidationFailed = null;
        WaitingForOtpValidation = null;
        OddDepositProcessing = null;
        OddDepositCompleted = null;
        OddDepositFailed = null;
        OtpValidationNotReceived = null;
    }
}
