using System.Linq.Expressions;
using MassTransit;

namespace Pi.WalletService.IntegrationEvents.Models;

public static class RecoveryState
{
    public static State? RevertRequestReceived { get; set; }
    public static State? RevertTransferInitiate { get; set; }
    public static State? RevertingTransfer { get; set; }
    public static State? RevertTransferSuccess { get; set; }
    public static State? RevertTransferFailed { get; set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static void CleanUp()
    {
        RevertRequestReceived = null;
        RevertTransferInitiate = null;
        RevertingTransfer = null;
        RevertTransferSuccess = null;
        RevertTransferFailed = null;
    }
}
