using System.Linq.Expressions;
using MassTransit;

namespace Pi.Financial.FundService.IntegrationEvents.Models;

public static class FundOrderState
{
    public static State? GeneratingOrderNo { get; set; }
    public static State? SendingOrderToBroker { get; set; }
    public static State? OrderPlaced { get; set; }
    public static State? PlaceOrderFailed { get; set; }
    public static State? OrderCancelled { get; set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static void CleanUp()
    {
        GeneratingOrderNo = null;
        SendingOrderToBroker = null;
        OrderPlaced = null;
        PlaceOrderFailed = null;
        OrderCancelled = null;
    }
}
