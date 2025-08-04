using MassTransit;
using RecoveryState = Pi.WalletService.Domain.AggregatesModel.RecoveryAggregate.RecoveryState;

namespace Pi.WalletService.Infrastructure.SagaConfigs;

public class RecoveryStateSagaDefinitions : SagaDefinition<RecoveryState>
{
    private readonly IServiceProvider _provider;

    public RecoveryStateSagaDefinitions(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<RecoveryState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<WalletDbContext>(_provider);
    }
}
