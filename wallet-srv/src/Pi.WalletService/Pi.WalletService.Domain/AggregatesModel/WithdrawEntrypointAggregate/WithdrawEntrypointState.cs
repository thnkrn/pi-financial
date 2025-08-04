using MassTransit;
namespace Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;

public class WithdrawEntrypointState : BaseEntryPoint, SagaStateMachineInstance
{
    public Guid? GlobalManualAllocateId { get; set; }
}
