using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
namespace Pi.WalletService.Infrastructure.SagaConfigs;

public class CashDepositStateSagaDefinitions : SagaDefinition<CashDepositState>
{
    private readonly IServiceProvider _provider;

    public CashDepositStateSagaDefinitions(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<CashDepositState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<WalletDbContext>(_provider);
    }
}
