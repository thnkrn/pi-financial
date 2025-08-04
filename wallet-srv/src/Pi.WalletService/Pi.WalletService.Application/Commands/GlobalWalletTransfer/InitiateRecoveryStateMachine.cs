using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;

namespace Pi.WalletService.Application.Commands.GlobalWalletTransfer;

public record InitiateRecoveryStateMachine(Guid CorrelationId);

public record InitiateRecoveryStateMachineSuccess(string GlobalAccount, Currency TransferCurrency, decimal TransferAmount);

public record InitiateRecoveryStateMachineFailed(Guid CorrelationId, string Reason);

public class InitiateRecoveryStateMachineConsumer : IConsumer<InitiateRecoveryStateMachine>
{
    private readonly IGlobalTransferRepository _globalTransferRepository;
    private readonly ILogger<InitiateRecoveryStateMachineConsumer> _logger;

    public InitiateRecoveryStateMachineConsumer(
        IGlobalTransferRepository globalTransferRepository,
        ILogger<InitiateRecoveryStateMachineConsumer> logger)
    {
        _globalTransferRepository = globalTransferRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InitiateRecoveryStateMachine> context)
    {
        var correlationId = context.Message.CorrelationId;
        try
        {
            var globalTransaction = await _globalTransferRepository.Get(correlationId);

            if (globalTransaction != null)
            {
                await context.RespondAsync(
                    new InitiateRecoveryStateMachineSuccess(
                        globalTransaction.GlobalAccount,
                        globalTransaction.TransferCurrency!.Value,
                        globalTransaction.TransferAmount!.Value
                    )
                );
            }
            else
            {
                await context.RespondAsync(
                    new InitiateRecoveryStateMachineFailed(
                        context.Message.CorrelationId,
                        "Request faulted or didn't return any data"
                    )
                );
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while initiating recovery state machine");
            await context.RespondAsync(new InitiateRecoveryStateMachineFailed(context.Message.CorrelationId, e.Message));
        }
    }
}
