namespace Pi.WalletService.Application.Models;

[Obsolete("Use TransactionV2 instead")]
public record WithdrawTransaction : BaseTransaction
{
    public required DateTime? PaymentDisbursedDateTime { get; init; }
    public required decimal? PaymentDisbursedAmount { get; init; }
    public required string? OtpRequestRef { get; init; }
    public required Guid? OtpRequestId { get; init; }
    public required DateTime? OtpConfirmedDateTime { get; init; }
}
