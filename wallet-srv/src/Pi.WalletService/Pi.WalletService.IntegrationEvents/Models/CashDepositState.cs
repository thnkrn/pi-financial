using System.Linq.Expressions;
using MassTransit;

namespace Pi.WalletService.IntegrationEvents.Models;

public static class CashDepositState
{
    public static State? Received { get; set; }
    public static State? CashDepositTradingPlatformUpdating { get; set; }
    public static State? CashDepositWaitingForGateway { get; set; }
    public static State? CashDepositWaitingForTradingPlatform { get; set; }
    public static State? TradingPlatformCashDepositFailed { get; set; }
    public static State? CashDepositPaymentNotReceived { get; set; }
    public static State? CashDepositCompleted { get; set; }
    public static State? CashDepositFailed { get; set; }
    public static State? TfexCashDepositFailed { get; set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }
    public static void CleanUp()
    {
        Received = null;
        CashDepositTradingPlatformUpdating = null;
        CashDepositWaitingForGateway = null;
        CashDepositWaitingForTradingPlatform = null;
        TradingPlatformCashDepositFailed = null;
        CashDepositPaymentNotReceived = null;
        CashDepositCompleted = null;
        CashDepositFailed = null;
        TfexCashDepositFailed = null;
    }
}
