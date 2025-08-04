using MassTransit;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;

/// <inheritdoc />
public class GlobalWalletTransferState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string? TransactionNo { get; set; }
    public TransactionType TransactionType { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long CustomerId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string GlobalAccount { get; set; } = string.Empty;
    public string? CurrentState { get; set; }
    public Product Product { get; set; }
    public decimal RequestedAmount { get; set; }
    public Currency RequestedCurrency { get; set; }
    public decimal RequestedFxAmount { get; set; }
    public Currency RequestedFxCurrency { get; set; }
    public decimal? PaymentReceivedAmount { get; set; }
    public Currency? PaymentReceivedCurrency { get; set; }
    public DateTime? FxInitiateRequestDateTime { get; set; }
    public string? FxTransactionId { get; set; }
    public decimal? FxConfirmedAmount { get; set; }
    public decimal? FxConfirmedExchangeRate { get; set; }
    public Currency? FxConfirmedCurrency { get; set; }
    public DateTime? FxConfirmedDateTime { get; set; }
    public string? TransferFromAccount { get; set; }
    public decimal? TransferAmount { get; set; }
    // Currency Should always be same with TransferCurrency
    public decimal? TransferFee { get; set; }
    public string? TransferToAccount { get; set; }
    public Currency? TransferCurrency { get; set; }
    public DateTime? TransferRequestTime { get; set; }
    public DateTime? TransferCompleteTime { get; set; }
    public string? FailedReason { get; set; }
    // Always in THB
    public decimal? RefundAmount { get; set; }
    // Always in THB
    public decimal? NetAmount { get; set; }
    // If using Optimistic concurrency, this property is required
    public byte[]? RowVersion { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string ResponseAddress { get; set; } = string.Empty;
    public Guid? RequestId { get; set; }
    public Guid? RequesterDeviceId { get; set; }
}