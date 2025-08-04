using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.SeedWork;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;

/// <inheritdoc />
public class GlobalTransferState : BaseEntity, SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public Product? Product { get; set; }
    public Channel? Channel { get; set; }
    public TransactionType TransactionType { get; set; }
    public long CustomerId { get; set; }
    public string GlobalAccount { get; set; } = string.Empty;
    public Currency RequestedCurrency { get; set; }
    public decimal RequestedFxRate { get; set; }
    public decimal? ActualFxRate { get; set; }
    public decimal? FxConfirmedExchangeRate { get; set; }
    public decimal FxMarkUpRate { get; set; }
    public Currency RequestedFxCurrency { get; set; }
    public decimal? ExchangeAmount { get; set; }
    public Currency? ExchangeCurrency { get; set; }
    public DateTime? FxInitiateRequestDateTime { get; set; }
    public string? FxTransactionId { get; set; }
    public decimal? FxConfirmedExchangeAmount { get; set; }
    public Currency? FxConfirmedExchangeCurrency { get; set; }
    public decimal? FxConfirmedAmount { get; set; }
    public Currency? FxConfirmedCurrency { get; set; }
    public DateTime? FxConfirmedDateTime { get; set; }
    public string? TransferFromAccount { get; set; }
    public decimal? TransferAmount { get; set; }
    public decimal? TransferFee { get; set; }
    public string? TransferToAccount { get; set; }
    public Currency? TransferCurrency { get; set; }
    public DateTime? TransferRequestTime { get; set; }
    public DateTime? TransferCompleteTime { get; set; }
    public string? FailedReason { get; set; }
    public Guid? RequestId { get; set; }
    public string ResponseAddress { get; set; } = string.Empty;
}
