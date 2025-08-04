using System.Linq.Expressions;
using MassTransit;

namespace Pi.WalletService.IntegrationEvents.Models;

public static class AtsDepositState
{
    public static State? Initial { get; set; }
    public static State? Received { get; set; }
    public static State? RequestingOtpValidation { get; set; }
    public static State? RequestingOtpValidationFailed { get; set; }
    public static State? WaitingForOtpValidation { get; set; }
    public static State? RequestingDepositAts { get; set; }
    public static State? WaitingForAtsGatewayConfirmation { get; set; }
    public static State? AtsDepositCompleted { get; set; }
    public static State? AtsDepositFailed { get; set; }
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
        RequestingDepositAts = null;
        WaitingForAtsGatewayConfirmation = null;
        AtsDepositCompleted = null;
        AtsDepositFailed = null;
        OtpValidationNotReceived = null;
    }
}
