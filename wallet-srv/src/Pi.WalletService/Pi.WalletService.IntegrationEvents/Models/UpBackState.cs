using System.Linq.Expressions;
using MassTransit;

namespace Pi.WalletService.IntegrationEvents.Models;

public static class UpBackState
{
    public static State? Initial { get; set; }
    public static State? DepositReceived { get; set; }
    public static State? WithdrawReceived { get; set; }
    public static State? DepositUpdatingAccountBalance { get; set; }
    public static State? DepositWaitingForGateway { get; set; }
    public static State? DepositUpdatingTradingPlatform { get; set; }
    public static State? WithdrawUpdatingAccountBalance { get; set; }
    public static State? WithdrawWaitingForGateway { get; set; }
    public static State? WithdrawUpdatingTradingPlatform { get; set; }
    public static State? UpBackCompleted { get; set; }
    public static State? UpBackFailed { get; set; }
    public static State? UpBackFailedRequireActionRevert { get; set; }
    public static State? UpBackFailedRequireActionSba { get; set; }
    public static State? UpBackFailedRequireActionSetTrade { get; set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static void CleanUp()
    {
        Initial = null;
        DepositReceived = null;
        WithdrawReceived = null;
        DepositUpdatingAccountBalance = null;
        DepositWaitingForGateway = null;
        DepositUpdatingTradingPlatform = null;
        WithdrawUpdatingAccountBalance = null;
        WithdrawWaitingForGateway = null;
        WithdrawUpdatingTradingPlatform = null;
        UpBackCompleted = null;
        UpBackFailed = null;
        UpBackFailedRequireActionRevert = null;
        UpBackFailedRequireActionSba = null;
        UpBackFailedRequireActionSetTrade = null;
    }
}
