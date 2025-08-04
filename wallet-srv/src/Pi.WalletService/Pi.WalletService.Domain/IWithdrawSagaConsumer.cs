using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
namespace Pi.WalletService.Domain;

public interface IWithdrawSagaConsumer
{
    Task<WithdrawEntrypointState?> GetWithdrawEntrypointByIdAsync(Guid correlationId, CancellationToken cancellationToken = default(CancellationToken));
}
