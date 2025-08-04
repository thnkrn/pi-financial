using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;

namespace Pi.WalletService.Infrastructure.SagaConfigs;

public class AtsDepositStateSagaDefinitions : SagaDefinition<AtsDepositState>
{
    private readonly IServiceProvider _provider;

    public AtsDepositStateSagaDefinitions(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<AtsDepositState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<WalletDbContext>(_provider);
    }
}
