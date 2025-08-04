using System.Linq.Expressions;
using MassTransit;
namespace Pi.WalletService.IntegrationEvents.Models;

public static class WithdrawEntrypointState
{
    public static State? Received { get; set; }
    public static State? Initiate { get; set; }
    public static State? TransactionNoGenerating { get; set; }
    public static State? WithdrawValidating { get; set; }
    public static State? UpBackProcessing { get; set; }
    public static State? GlobalTransferProcessing { get; set; }
    public static State? WithdrawProcessing { get; set; }
    public static State? WithdrawSucceed { get; set; }
    public static State? WithdrawFailedRequireActionRecovery { get; set; }
    public static State? WithdrawFailed { get; set; }
    public static State? OtpValidationNotReceived { get; set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static void CleanUp()
    {
        Received = null;
        Initiate = null;
        TransactionNoGenerating = null;
        WithdrawValidating = null;
        UpBackProcessing = null;
        GlobalTransferProcessing = null;
        WithdrawProcessing = null;
        WithdrawSucceed = null;
        WithdrawFailedRequireActionRecovery = null;
        WithdrawFailed = null;
        OtpValidationNotReceived = null;
    }
}
