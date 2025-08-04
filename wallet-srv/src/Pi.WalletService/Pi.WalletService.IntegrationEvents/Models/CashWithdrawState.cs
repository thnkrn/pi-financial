using System.Linq.Expressions;
using MassTransit;

namespace Pi.WalletService.IntegrationEvents.Models;

public static class CashWithdrawState
{
    public static State? Received { get; private set; }
    public static State? TransactionNoGenerating { get; private set; }
    public static State? CashWithdrawWaitingForOtpValidation { get; private set; }
    public static State? CashWithdrawWaitingForTradingPlatform { get; private set; }
    public static State? CashWithdrawWaitingForTFexPlatform { get; private set; }
    public static State? CashWithdrawTradingPlatformUpdating { get; private set; }
    public static State? WithdrawalProcessing { get; private set; }
    public static State? TransferRequestFailed { get; private set; }
    public static State? RevertTfexTransfer { get; private set; }
    public static State? RevertPlatformTransfer { get; private set; }
    public static State? RevertWaitingForPlatformCallback { get; private set; }
    public static State? RevertTransferSuccess { get; private set; }
    public static State? RevertTransferFailed { get; private set; }
    public static State? WithdrawalFailed { get; private set; }
    public static State? CashWithdrawOtpValidationNotReceived { get; private set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }
    public static void CleanUp()
    {
        Received = null;
        TransactionNoGenerating = null;
        CashWithdrawWaitingForOtpValidation = null;
        CashWithdrawWaitingForTradingPlatform = null;
        CashWithdrawWaitingForTFexPlatform = null;
        CashWithdrawTradingPlatformUpdating = null;
        WithdrawalProcessing = null;
        TransferRequestFailed = null;
        RevertTfexTransfer = null;
        RevertPlatformTransfer = null;
        RevertTransferSuccess = null;
        RevertTransferFailed = null;
        RevertWaitingForPlatformCallback = null;
        WithdrawalFailed = null;
        CashWithdrawOtpValidationNotReceived = null;
    }
}
