using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;

namespace Pi.WalletService.Application.Models;

[Obsolete("Use TransactionV2 instead")]
public record DepositTransaction : BaseTransaction
{
    public required Purpose Purpose { get; init; }
    public required decimal RequestedAmount { get; init; }
    public required decimal? ReceivedAmount { get; init; }
    public required DateTime? PaymentReceivedDateTime { get; init; }
    public required decimal? PaymentReceivedAmount { get; init; }
    public required string? CustomerName { get; init; }
    public required string? BankAccountName { get; init; }
    public required DateTime? QrGenerateDateTime { get; init; }
    public required DateTime? QrExpiredTime { get; init; }
    public required int QrCodeExpiredTimeInMinute { get; init; }
    public required string? QrTransactionNo { get; init; }
    public required string? QrValue { get; init; }
    public required string? QrTransactionRef { get; init; }
}
