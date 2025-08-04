using System.Linq.Expressions;
using MassTransit;
namespace Pi.WalletService.IntegrationEvents.Models;

public class AtsWithdrawState
{
    public static State? Initial { get; set; }
    public static State? Received { get; set; }
    public static State? RequestingOtpValidation { get; set; }
    public static State? RequestingOtpValidationFailed { get; set; }
    public static State? WaitingForOtpValidation { get; set; }
    public static State? WaitingForConfirmation { get; set; }
    public static State? RequestingWithdrawAts { get; set; }
    public static State? WaitingForAtsGatewayConfirmation { get; set; }
    public static State? AtsWithdrawCompleted { get; set; }
    public static State? AtsWithdrawFailed { get; set; }
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
        WaitingForConfirmation = null;
        RequestingWithdrawAts = null;
        WaitingForAtsGatewayConfirmation = null;
        AtsWithdrawCompleted = null;
        AtsWithdrawFailed = null;
        OtpValidationNotReceived = null;
    }
}