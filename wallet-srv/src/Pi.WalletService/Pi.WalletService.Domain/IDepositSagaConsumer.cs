using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
namespace Pi.WalletService.Domain;

public interface IDepositSagaConsumer
{
    Task<DepositEntrypointState?> GetDepositEntrypointByIdAsync(Guid correlationId, CancellationToken cancellationToken = default(CancellationToken));

}
