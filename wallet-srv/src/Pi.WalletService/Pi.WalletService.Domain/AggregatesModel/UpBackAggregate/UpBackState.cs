using MassTransit;
using Pi.WalletService.Domain.SeedWork;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;

public class UpBackState : BaseEntity, SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public Product? Product { get; set; }
    public Channel? Channel { get; set; }
    public TransactionType TransactionType { get; set; }
    public string? FailedReason { get; set; }
}
