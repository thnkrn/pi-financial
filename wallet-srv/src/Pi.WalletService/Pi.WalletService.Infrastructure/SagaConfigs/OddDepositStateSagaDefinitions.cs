using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;

namespace Pi.WalletService.Infrastructure.SagaConfigs;

public class OddDepositStateSagaDefinitions : SagaDefinition<OddDepositState>
{
    private readonly IServiceProvider _provider;

    public OddDepositStateSagaDefinitions(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<OddDepositState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<WalletDbContext>(_provider);
    }
}
