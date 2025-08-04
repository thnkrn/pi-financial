using System.Linq.Expressions;
using MassTransit;

namespace Pi.WalletService.IntegrationEvents.Models;

public static class DepositEntrypointState
{
    public static State? Received { get; set; }
    public static State? Initiate { get; set; }
    public static State? TransactionNoGenerating { get; set; }
    public static State? DepositProcessing { get; set; }
    public static State? UpBackProcessing { get; set; }
    public static State? GlobalTransferProcessing { get; set; }
    public static State? DepositFailed { get; set; }
    public static State? DepositPaymentNotReceived { get; set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static IEnumerable<State?> GetRefundAllowedStates()
    {
        return new[] { DepositProcessing, UpBackProcessing, GlobalTransferProcessing };
    }

    public static void CleanUp()
    {
        Received = null;
        Initiate = null;
        TransactionNoGenerating = null;
        DepositProcessing = null;
        UpBackProcessing = null;
        GlobalTransferProcessing = null;
        DepositFailed = null;
        DepositPaymentNotReceived = null;
    }
}
