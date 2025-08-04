using System.Linq.Expressions;
using MassTransit;
namespace Pi.WalletService.IntegrationEvents.Models;

public static class GlobalManualAllocationState
{
    public static State? Allocating { get; set; }
    public static State? Failed { get; set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static void CleanUp()
    {
        Allocating = null;
        Failed = null;
    }
}
