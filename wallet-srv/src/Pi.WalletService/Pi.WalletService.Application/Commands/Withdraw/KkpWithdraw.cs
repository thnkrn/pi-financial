using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Services.PaymentService;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Withdraw;

public record KkpWithdrawRequest(
    string AccountCode,
    decimal Amount,
    string BankAccountNo,
    string BankCode,
    Product Product,
    string? TransactionNo
);

public class KkpWithdrawConsumer :
    WithdrawSagaConsumer,
    IConsumer<KkpWithdrawRequest>
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<KkpWithdrawConsumer> _logger;

    public KkpWithdrawConsumer(
        IPaymentService paymentService,
        IWithdrawEntrypointRepository withdrawEntrypointRepository, ILogger<KkpWithdrawConsumer> logger) : base(withdrawEntrypointRepository)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<KkpWithdrawRequest> context)
    {
        if (context.Message.Amount < 0)
        {
            throw new InvalidDataException("Withdraw Negative Amount");
        }

        var dateTimeNow = DateTime.Now;
        var transactionNo = context.Message.TransactionNo ??
                            $"DD{dateTimeNow.ToString("yyyyMMddHHmmssffff", CultureInfo.InvariantCulture)}";

        _logger.LogInformation("KkpWithdrawRequest Calling Payment Service TransferViaOdd");

        var response = await _paymentService.TransferViaOdd(
            transactionNo,
            context.Message.Amount,
            TransactionType.Withdraw,
            context.Message.BankCode,
            context.Message.BankAccountNo,
            string.Empty,
            string.Empty,
            context.Message.AccountCode,
            context.Message.Product
        );

        await context.RespondAsync(
            new WithdrawOddSucceed(
                transactionNo,
                response,
                dateTimeNow
            )
        );
    }
}