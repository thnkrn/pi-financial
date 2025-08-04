using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Domain.Exceptions;
namespace Pi.WalletService.Application.Commands.Deposit;

public record ValidatePaymentAmount(decimal RequestedAmount, decimal PaymentAmount);
public record ValidatePaymentAmountV2(Guid CorrelationId, decimal PaymentAmount);

public class ValidatePaymentAmountConsumer :
    SagaConsumer,
    IConsumer<ValidatePaymentAmountV2>,
    IConsumer<ValidatePaymentAmount>
{

    private readonly ILogger<ValidatePaymentAmountConsumer> _logger;
    private readonly IDepositEntrypointRepository _depositEntrypointRepository;
    private readonly IWithdrawEntrypointRepository _withdrawEntrypointRepository;

    public ValidatePaymentAmountConsumer(
        ILogger<ValidatePaymentAmountConsumer> logger,
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository) : base(depositEntrypointRepository, withdrawEntrypointRepository)
    {
        _logger = logger;
        _depositEntrypointRepository = depositEntrypointRepository;
        _withdrawEntrypointRepository = withdrawEntrypointRepository;
    }

    public async Task Consume(ConsumeContext<ValidatePaymentAmount> context)
    {
        if (decimal.Equals(context.Message.RequestedAmount, context.Message.PaymentAmount))
        {
            await context.RespondAsync(new DepositValidatePaymentAmountSucceed(context.Message.PaymentAmount));
        }
        else
        {
            throw new InvalidDepositAmountException($"Deposit amount mismatched, requested {context.Message.RequestedAmount:N2}, got {context.Message.PaymentAmount:N2}");
        }
    }

    public async Task Consume(ConsumeContext<ValidatePaymentAmountV2> context)
    {
        var depositEntrypointState = await GetDepositEntrypointByIdAsync(context.Message.CorrelationId);
        if (depositEntrypointState == null)
        {
            throw new Exception($"Deposit Entrypoint Not Found");
        }

        if (!decimal.Equals(depositEntrypointState.RequestedAmount, context.Message.PaymentAmount))
        {
            throw new InvalidDepositAmountException($"Deposit Amount Mismatched, Requested {depositEntrypointState.RequestedAmount:N2}, Got {context.Message.PaymentAmount:N2}");
        }

        await context.RespondAsync(new DepositValidatePaymentAmountSucceed(context.Message.PaymentAmount));
    }
}
