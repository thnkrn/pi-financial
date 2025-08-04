using Pi.WalletService.Application.Queries;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Models;

[Obsolete("Use TransactionV2 instead")]
public record Transaction(
    Guid Id,
    string UserId,
    string AccountNo,
    string CustomerCode,
    string? CustomerName,
    string? CurrentState,
    string Status,
    string? TransactionNo,
    TransactionType TransactionType,
    // Deposit will be Request Amount, Withdraw will be PaymentDisbursedAmount
    decimal? Amount,
    Currency Currency,
    string? QrValue,
    DateTime? QrGenerateDateTime,
    decimal? ReceivedAmount,
    DateTime? PaymentReceivedDateTime,
    DateTime? PaymentDisbursedDateTime,
    Channel Channel,
    Product Product,
    string? BankAccountNo,
    string? BankAccountName,
    string? BankName,
    decimal? BankFee,
    DateTime CreatedAt,
    // For Withdraw Confirmed Amount without Fee
    decimal? ConfirmedAmount
);

[Obsolete("Use TransactionV2 instead")]
public abstract record BaseTransaction
{
    public required Guid Id { get; init; }
    public required string UserId { get; init; } = string.Empty;
    public required string? TransactionNo { get; init; }
    public required string AccountCode { get; init; } = string.Empty;
    public required string CustomerCode { get; init; } = string.Empty;
    public required decimal? Amount { get; init; }
    public required Product Product { get; init; }
    public required TransactionType TransactionType { get; init; }
    public required string? CurrentState { get; init; }
    public required decimal? BankFee { get; init; }
    public required string? BankName { get; init; }
    public required TransactionStatus Status { get; init; }
    public required string? BankCode { get; init; }
    public required string? BankAccountNo { get; init; }
    public required DateTime? EffectiveDateTime { get; init; }
    public required Currency Currency { get; init; }
    public required Channel Channel { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string? FailedReason { get; init; }
}
