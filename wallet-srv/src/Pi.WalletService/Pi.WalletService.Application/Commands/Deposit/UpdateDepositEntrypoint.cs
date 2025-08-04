using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
namespace Pi.WalletService.Application.Commands.Deposit;

public record UpdateDepositEntrypointRequest(DepositEntrypointState EntrypointState);

public record UpdateDepositEntrypointSuccess(Guid CorrelationId);

public class UpdateEntrypointConsumer :
    SagaConsumer,
    IConsumer<UpdateDepositEntrypointRequest>
{
    private readonly IDepositEntrypointRepository _depositEntrypointRepository;
    private readonly ILogger<UpdateEntrypointConsumer> _logger;

    public UpdateEntrypointConsumer(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        ILogger<UpdateEntrypointConsumer> logger) : base(depositEntrypointRepository, withdrawEntrypointRepository)
    {
        _depositEntrypointRepository = depositEntrypointRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UpdateDepositEntrypointRequest> context)
    {
        try
        {
            _logger.LogInformation("Before update payment received data: {CorrelationId}", context.Message.EntrypointState.CorrelationId);
            await _depositEntrypointRepository.UpdatePaymentReceivedData(context.Message.EntrypointState);
            await context.RespondAsync(new UpdateDepositEntrypointSuccess(context.Message.EntrypointState.CorrelationId));
            _logger.LogInformation("After update payment received data: {CorrelationId}", context.Message.EntrypointState.CorrelationId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "UpdateDepositEntrypointConsumer");
            throw;
        }
    }
}
