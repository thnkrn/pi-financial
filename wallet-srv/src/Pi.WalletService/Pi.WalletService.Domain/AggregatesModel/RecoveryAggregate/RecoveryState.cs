using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.SeedWork;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.RecoveryAggregate;

public class RecoveryState : BaseEntity, SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public Product? Product { get; set; }
    public TransactionType? TransactionType { get; set; }
    public string GlobalAccount { get; set; } = string.Empty;
    public string? TransferFromAccount { get; set; }
    public decimal? TransferAmount { get; set; }
    public string? TransferToAccount { get; set; }
    public Currency? TransferCurrency { get; set; }
    public DateTime? TransferRequestTime { get; set; }
    public DateTime? TransferCompleteTime { get; set; }
    public string? FailedReason { get; set; }
    public Guid? RequestId { get; set; }
    public string ResponseAddress { get; set; } = string.Empty;
}
