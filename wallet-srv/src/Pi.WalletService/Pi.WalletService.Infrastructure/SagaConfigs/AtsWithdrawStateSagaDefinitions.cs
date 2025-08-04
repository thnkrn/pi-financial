using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.AtsWithdrawAggregate;

namespace Pi.WalletService.Infrastructure.SagaConfigs;

public class AtsWithdrawStateSagaDefinitions : SagaDefinition<AtsWithdrawState>
{
    private readonly IServiceProvider _provider;

    public AtsWithdrawStateSagaDefinitions(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<AtsWithdrawState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<WalletDbContext>(_provider);
    }
}
