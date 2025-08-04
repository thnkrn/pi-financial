using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Models;

public record MetricTags(Product? Product, Channel? Channel, TagFailedReason? FailedReason, TransactionType? TransactionType)
{
    public KeyValuePair<string, object?>[] GetValues()
    {
        var tags = new Dictionary<string, object?>();

        if (Product != null) tags.Add("product", Product.ToString());
        if (Channel != null) tags.Add("channel", Channel.ToString());
        if (FailedReason != null) tags.Add("failed_reason", FailedReason.ToString());
        if (TransactionType != null) tags.Add("transaction_type", TransactionType.ToString());

        return tags.ToArray();
    }
}

public static class Metrics
{

    #region common

    public const string WithdrawReceived = "withdraw.received";
    public const string WithdrawFailed = "withdraw.failed";
    public const string WithdrawSuccess = "withdraw.success";
    public const string WithdrawAmount = "withdraw.amount";
    public const string WithdrawCancelled = "withdraw.cancelled";

    public const string DepositReceived = "deposit.received";
    public const string DepositFailed = "deposit.failed";
    public const string DepositSuccess = "deposit.success";
    public const string DepositRefundSuccess = "deposit.refund.success";
    public const string DepositRefundFailed = "deposit.refund.failed";
    public const string DepositAmount = "deposit.amount";
    public const string DepositCancelled = "deposit.cancelled";

    public const string DepositOddReceived = "odd.deposit.received";
    public const string DepositOddFailed = "odd.deposit.failed";
    public const string DepositOddSuccess = "odd.deposit.success";

    public const string DepositAtsReceived = "ats.deposit.received";
    public const string DepositAtsSuccess = "ats.deposit.success";
    public const string DepositAtsFailed = "ats.deposit.failed";

    public const string WithdrawOddReceived = "odd.withdraw.received";
    public const string WithdrawOddFailed = "odd.withdraw.failed";
    public const string WithdrawOddSuccess = "odd.withdraw.success";

    public const string WithdrawAtsReceived = "ats.withdraw.received";
    public const string WithdrawAtsSuccess = "ats.withdraw.success";
    public const string WithdrawAtsFailed = "ats.withdraw.failed";


    public const string RefundReceived = "refund.received";
    public const string RefundSuccess = "refund.success";
    public const string RefundFailed = "refund.failed";

    #endregion

    #region global

    public const string GlobalManualAllocationSuccess = "global.manual-allocation.success";
    public const string GlobalManualAllocationFailed = "global.manual-allocation.failed";

    public const string GlobalRequestReceived = "global.request.received";
    public const string GlobalRequestFailed = "global.request.failed";
    public const string GlobalRequestSuccess = "global.request.success";

    public const string GlobalTransferReceived = "global.upback.received";
    public const string GlobalTransferSuccess = "global.upback.success";
    public const string GlobalTransferFailed = "global.upback.failed";

    public const string GlobalRefundSuccess = "global.refund.success";
    public const string GlobalRefundFailed = "global.refund.failed";

    public const string RevertReceived = "revert.received";
    public const string RevertSuccess = "revert.success";
    public const string RevertFailed = "revert.failed";

    #endregion

    #region non-global

    public const string QrDepositReceived = "qr.deposit.received";
    public const string QrDepositAmount = "qr.deposit.amount";
    public const string QrDepositSuccess = "qr.deposit.success";
    public const string QrDepositFailed = "qr.deposit.failed";
    public const string QrDepositExpired = "qr.deposit.expired";

    public const string UpBackReceived = "upback.received";
    public const string UpBackSuccess = "upback.success";
    public const string UpBackFailed = "upback.failed";
    public const string UpBackFailedRequiredActionSba = "upback.requiredaction.sba";
    public const string UpBackFailedRequiredActionSetTrade = "upback.requiredaction.settrade";

    public const string CashDepositReceived = "cash.deposit.received";
    public const string CashDepositSuccess = "cash.deposit.success";
    public const string CashDepositFailed = "cash.deposit.failed";
    public const string CashDepositAmount = "cash.deposit.amount";

    public const string CashWithdrawReceived = "cash.withdraw.received";
    public const string CashWithdrawSuccess = "cash.withdraw.success";
    public const string CashWithdrawRevertTransferFailed = "cash.withdraw.revert.failed";
    public const string CashWithdrawRevertTransferSuccess = "cash.withdraw.revert.success";
    public const string CashWithdrawFailed = "cash.withdraw.failed";
    public const string CashWithdrawAmount = "cash.withdraw.amount";

    #endregion

}
