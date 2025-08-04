using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;

namespace Pi.WalletService.Infrastructure.SagaConfigs;

public class GlobalTransferStateSagaDefinitions : SagaDefinition<GlobalTransferState>
{
    private readonly IServiceProvider _provider;

    public GlobalTransferStateSagaDefinitions(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<GlobalTransferState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<WalletDbContext>(_provider);
    }
}
