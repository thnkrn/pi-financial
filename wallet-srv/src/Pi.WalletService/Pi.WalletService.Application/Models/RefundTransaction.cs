namespace Pi.WalletService.Application.Models;

[Obsolete("Use TransactionV2 instead")]
public record RefundTransaction : BaseTransaction
{
    public required string? DepositTransactionNo { get; set; }
    public required DateTime? RefundedAt { get; set; }
}
