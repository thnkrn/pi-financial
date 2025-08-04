using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
namespace Pi.WalletService.Infrastructure.SagaConfigs;

public class DepositStateSagaDefinitions : SagaDefinition<DepositState>
{
    private readonly IServiceProvider _provider;

    public DepositStateSagaDefinitions(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<DepositState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<WalletDbContext>(_provider);
    }
}
