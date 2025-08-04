using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
namespace Pi.WalletService.Infrastructure.SagaConfigs;

public class DepositEntrypointStateSagaDefinitions : SagaDefinition<DepositEntrypointState>
{
    private readonly IServiceProvider _provider;

    public DepositEntrypointStateSagaDefinitions(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<DepositEntrypointState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<WalletDbContext>(_provider);
    }
}
