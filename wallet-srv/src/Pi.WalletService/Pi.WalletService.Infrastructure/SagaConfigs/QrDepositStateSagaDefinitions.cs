using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
namespace Pi.WalletService.Infrastructure.SagaConfigs;

public class QrDepositStateSagaDefinitions : SagaDefinition<QrDepositState>
{
    private readonly IServiceProvider _provider;

    public QrDepositStateSagaDefinitions(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<QrDepositState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<WalletDbContext>(_provider);
    }
}
