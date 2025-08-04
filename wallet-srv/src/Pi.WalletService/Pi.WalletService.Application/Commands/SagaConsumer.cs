using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
namespace Pi.WalletService.Application.Commands;

public class SagaConsumer : IDepositSagaConsumer, IWithdrawSagaConsumer
{
    private readonly IDepositEntrypointRepository _depositEntrypointRepository;
    private readonly IWithdrawEntrypointRepository _withdrawEntrypointRepository;

    protected SagaConsumer(IDepositEntrypointRepository depositEntrypointRepository, IWithdrawEntrypointRepository withdrawEntrypointRepository)
    {
        _depositEntrypointRepository = depositEntrypointRepository;
        _withdrawEntrypointRepository = withdrawEntrypointRepository;
    }

    public async Task<DepositEntrypointState?> GetDepositEntrypointByIdAsync(Guid correlationId, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await _depositEntrypointRepository.GetById(correlationId);
    }

    public async Task<WithdrawEntrypointState?> GetWithdrawEntrypointByIdAsync(Guid correlationId, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await _withdrawEntrypointRepository.GetById(correlationId);
    }

    public async Task<BaseEntryPoint?> GetAnyEntryPointByIdAsync(Guid correlationId, CancellationToken cancellationToken = default(CancellationToken))
    {
        var deposit = await _depositEntrypointRepository.GetById(correlationId);
        if (deposit != null)
        {
            return deposit;
        }
        return await _withdrawEntrypointRepository.GetById(correlationId);
    }
}
