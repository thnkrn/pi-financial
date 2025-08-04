using System.Linq.Expressions;
using MassTransit;

namespace Pi.WalletService.IntegrationEvents.Models;

public static class RefundState
{
    public static State? Received { get; private set; }
    public static State? Refunding { get; private set; }
    public static State? RefundSucceed { get; private set; }
    public static State? RefundFailed { get; private set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static void CleanUp()
    {
        Received = null;
        Refunding = null;
        RefundSucceed = null;
        RefundFailed = null;
    }
}
