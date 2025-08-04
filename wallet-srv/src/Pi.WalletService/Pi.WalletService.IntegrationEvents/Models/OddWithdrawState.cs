using System.Linq.Expressions;
using MassTransit;
namespace Pi.WalletService.IntegrationEvents.Models;

public class OddWithdrawState
{
    public static State? Initial { get; set; }
    public static State? Received { get; set; }
    public static State? WithdrawalInitiating { get; set; }
    public static State? RequestingOtpValidation { get; set; }
    public static State? RequestingOtpValidationFailed { get; set; }
    public static State? WaitingForOtpValidation { get; set; }
    public static State? WaitingForConfirmation { get; set; }
    public static State? OddWithdrawProcessing { get; set; }
    public static State? OddWithdrawCompleted { get; set; }
    public static State? OddWithdrawFailed { get; set; }
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
        WithdrawalInitiating = null;
        RequestingOtpValidation = null;
        RequestingOtpValidationFailed = null;
        WaitingForOtpValidation = null;
        WaitingForConfirmation = null;
        OddWithdrawProcessing = null;
        OddWithdrawCompleted = null;
        OddWithdrawFailed = null;
        OtpValidationNotReceived = null;
    }
}
