using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
namespace Pi.WalletService.Domain.AggregatesModel.GlobalWalletManualAllocationAggregate;

public enum GlobalManualAllocationType
{
    Manual,
    DepositInsufficientBalanceAutoRetry
}

/// <inheritdoc />
public class GlobalManualAllocationState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public Guid TransactionId { get; set; }
    public string TransactionNo { get; set; } = string.Empty;
    public string GlobalAccount { get; set; } = string.Empty;
    public Currency Currency { get; set; }
    public decimal Amount { get; set; }
    public DateTime? InitiateTransferAt { get; set; }
    public DateTime? CompletedTransferAt { get; set; }

    // If using Optimistic concurrency, this property is required
    public byte[]? RowVersion { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string ResponseAddress { get; set; } = string.Empty;
    public GlobalManualAllocationType RequestType { get; set; }
    public Guid? RequestId { get; set; }
    public string? FailedReason { get; set; }
}