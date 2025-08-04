using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
namespace Pi.WalletService.Infrastructure.SagaConfigs;

public class UpBackStateSagaDefinitions : SagaDefinition<UpBackState>
{
    private readonly IServiceProvider _provider;

    public UpBackStateSagaDefinitions(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<UpBackState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<WalletDbContext>(_provider);
    }
}
