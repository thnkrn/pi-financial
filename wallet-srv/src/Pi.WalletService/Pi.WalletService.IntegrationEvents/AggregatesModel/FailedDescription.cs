namespace Pi.WalletService.IntegrationEvents.AggregatesModel;

public static class FailedDescription
{
    public const string ManualAllocationXnt = "Manual allocation in XNT";
    public const string ManualAllocationSba = "Manual allocation in SBA";
    public const string ManualAllocationSetTrade = "Manual allocation in SetTrade";
    public const string ApproveAts = "Pending ATS Approval";
    public const string InsufficientBalance = "Insufficient Balance";
    public const string AmountMismatch = "Expect Amount and Received Amount Mismatch";
    public const string NameMismatch = "Name Mismatch";
    public const string IncorrectSource = "Incorrect Source";
    public const string RevertUpBackTransaction = "Revert SBA or SetTrade Transaction";
    public const string UnableToFx = "Unable to fx";
    public const string FxRateOver = "Unfavorable fx (rate over)";
    public const string RefundSuccess = "Refund Success";
    public const string RevertTransaction = "Pending Revert Transaction";
}