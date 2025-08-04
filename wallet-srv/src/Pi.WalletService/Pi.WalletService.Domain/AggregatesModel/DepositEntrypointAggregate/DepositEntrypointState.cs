using MassTransit;

namespace Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;

public class DepositEntrypointState : BaseEntryPoint, SagaStateMachineInstance
{
    public Guid? RefundId { get; set; }
    public Guid? GlobalManualAllocateId { get; set; }
}