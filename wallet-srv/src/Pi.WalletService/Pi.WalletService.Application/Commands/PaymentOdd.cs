using MassTransit;
using Pi.WalletService.Application.Services.PaymentService;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.OddDeposit;
using Pi.WalletService.Domain.Events.OddWithdraw;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands;

public record PaymentDepositOddRequest(
    Guid CorrelationId
);

public record PaymentWithdrawRequest(
    Guid CorrelationId,
    decimal Amount
);

public class OddConsumer :
    SagaConsumer,
    IConsumer<PaymentDepositOddRequest>,
    IConsumer<PaymentWithdrawRequest>
{
    private readonly IPaymentService _paymentService;

    public OddConsumer(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        IPaymentService paymentService
        ) : base(depositEntrypointRepository, withdrawEntrypointRepository)
    {
        _paymentService = paymentService;
    }

    public async Task Consume(ConsumeContext<PaymentDepositOddRequest> context)
    {
        var depositEntrypoint = await GetDepositEntrypointByIdAsync(context.Message.CorrelationId);
        if (depositEntrypoint == null)
        {
            throw new InvalidDataException("Deposit Entrypoint Not Found");
        }

        if (depositEntrypoint.RequestedAmount < 1)
        {
            throw new InvalidDataException("Deposit Negative Amount");
        }

        var dateTimeNow = DateTime.Now;
        var refCode = await _paymentService.TransferViaOdd(
            depositEntrypoint.TransactionNo!,
            depositEntrypoint.RequestedAmount,
            TransactionType.Deposit,
            depositEntrypoint.BankCode!,
            depositEntrypoint.BankAccountNo!,
            depositEntrypoint.CustomerName!,
            depositEntrypoint.BankAccountTaxId!,
            "",
            depositEntrypoint.Product
        );

        await context.RespondAsync(
            new OddDepositSucceed(
                context.Message.CorrelationId,
                depositEntrypoint.UserId,
                refCode,
                depositEntrypoint.RequestedAmount,
                dateTimeNow,
                // If we want to add fee, put it here
                0
            )
        );
    }

    public async Task Consume(ConsumeContext<PaymentWithdrawRequest> context)
    {
        var withdrawEntrypoint = await GetWithdrawEntrypointByIdAsync(context.Message.CorrelationId);
        if (withdrawEntrypoint == null)
        {
            throw new InvalidDataException("Withdraw Entrypoint Not Found");
        }

        var dateTimeNow = DateTime.Now;
        var refCode = await _paymentService.TransferViaOdd(
            withdrawEntrypoint.TransactionNo!,
            context.Message.Amount,
            TransactionType.Withdraw,
            withdrawEntrypoint.BankCode!,
            withdrawEntrypoint.BankAccountNo!,
            string.Empty,
            string.Empty,
            withdrawEntrypoint.AccountCode,
            withdrawEntrypoint.Product
        );

        await context.RespondAsync(
            new OddWithdrawSucceed(
                context.Message.CorrelationId,
                refCode,
                dateTimeNow
            )
        );
    }
}
