using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.OddWithdrawAggregate;

namespace Pi.WalletService.Infrastructure.SagaConfigs;

public class OddWithdrawStateSagaDefinitions : SagaDefinition<OddWithdrawState>
{
    private readonly IServiceProvider _provider;

    public OddWithdrawStateSagaDefinitions(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<OddWithdrawState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<WalletDbContext>(_provider);
    }
}
