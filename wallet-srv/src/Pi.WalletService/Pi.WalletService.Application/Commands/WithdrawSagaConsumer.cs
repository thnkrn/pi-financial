using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
namespace Pi.WalletService.Application.Commands;

public class WithdrawSagaConsumer : IWithdrawSagaConsumer
{
    private readonly IWithdrawEntrypointRepository _withdrawEntrypointRepository;

    public WithdrawSagaConsumer(IWithdrawEntrypointRepository withdrawEntrypointRepository)
    {
        _withdrawEntrypointRepository = withdrawEntrypointRepository;
    }

    public async Task<WithdrawEntrypointState?> GetWithdrawEntrypointByIdAsync(Guid correlationId, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await _withdrawEntrypointRepository.GetById(correlationId);
    }
}
