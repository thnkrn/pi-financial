using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Application.Models;

public record Transaction()
{
    public required Guid Id { get; init; }
    public required string AccountNo { get; init; }
    public required string TransactionNo { get; init; }
    public required decimal? Amount { get; init; }
    public required Currency? Currency { get; init; }
    public required string Status { get; init; }
    public required string CustomerCode { get; init; }
    public required Product Product { get; init; }
    public required TransactionType TransactionType { get; init; }
    public required string? BankAccountNo { get; init; }
    public required string? BankName { get; init; }
    public required string UserId { get; init; }
    public required string? FailedReason { get; init; }
    public required DateTime? EffectiveDate { get; init; }
    public required DateTime CreatedAt { get; init; }
    public GlobalTransfer? GlobalTransfer { get; init; }
}

public record WithdrawTransaction() : Transaction
{
    public required WithdrawChannel? Channel { get; init; }
    public required DateTime? PaymentDisbursedDateTime { get; init; }
    public required decimal? PaymentDisbursedAmount { get; init; }
    public required string? CustomerName { get; init; }
}

public record DepositTransaction() : Transaction
{
    public required DepositChannel? Channel { get; init; }
    public required decimal RequestedAmount { get; init; }
    public required decimal? ReceivedAmount { get; init; }
    public required DateTime? PaymentReceivedDateTime { get; init; }
    public required string CustomerName { get; init; }
    public required string? BankAccountName { get; init; }
    public required DateTime? RefundAt { get; init; }
}
