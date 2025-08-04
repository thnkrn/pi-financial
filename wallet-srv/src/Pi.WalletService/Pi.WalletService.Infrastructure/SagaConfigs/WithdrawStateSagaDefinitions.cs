using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletManualAllocationAggregate;
using Pi.WalletService.Domain.AggregatesModel.RefundAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
namespace Pi.WalletService.Infrastructure.SagaConfigs;

public class WithdrawStateSagaDefinitions : SagaDefinition<WithdrawState>
{
    private readonly IServiceProvider _provider;

    public WithdrawStateSagaDefinitions(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<WithdrawState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<WalletDbContext>(_provider);
    }
}
