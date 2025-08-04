using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.GlobalWalletTransfer;

public record ValidateGlobalTransferDepositPayment(
    string TransactionNo,
    DateTime QrCodeGenerateDateTime,
    DateTime ReceivedPaymentDateTime,
    int ExpireTimeInMinute);

public record ValidateGlobalTransferDepositPaymentV2(Guid CorrelationId);

public class ValidateGlobalTransferPaymentConsumer :
    SagaConsumer,
    IConsumer<ValidateGlobalTransferDepositPaymentV2>,
    IConsumer<ValidateGlobalTransferDepositPayment>
{
    private readonly IQrDepositRepository _qrDepositRepository;
    private readonly ILogger<ValidateGlobalTransferPaymentConsumer> _logger;

    public ValidateGlobalTransferPaymentConsumer(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        IQrDepositRepository qrDepositRepository,
        ILogger<ValidateGlobalTransferPaymentConsumer> logger) : base(depositEntrypointRepository,
        withdrawEntrypointRepository)
    {
        _qrDepositRepository = qrDepositRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ValidateGlobalTransferDepositPayment> context)
    {
        var resp = ValidateGlobalTransferPayment(
            context.Message.TransactionNo,
            context.Message.ReceivedPaymentDateTime,
            context.Message.QrCodeGenerateDateTime,
            context.Message.ExpireTimeInMinute
        );

        if (resp is null)
        {
            throw new Exception("Deposit Entrypoint Not Found");
        }

        await context.RespondAsync(resp);
    }

    public async Task Consume(ConsumeContext<ValidateGlobalTransferDepositPaymentV2> context)
    {
        var depositEntrypoint = await GetDepositEntrypointByIdAsync(context.Message.CorrelationId);

        if (depositEntrypoint?.Channel is Channel.ODD or Channel.ATS)
        {
            await context.RespondAsync(
                new GlobalTransferDepositPaymentValidationCompleted(depositEntrypoint.TransactionNo!));
        }
        else
        {
            var qrDepositState =
                await _qrDepositRepository.GetAsync(ctx => ctx.CorrelationId == context.Message.CorrelationId);

            if (depositEntrypoint is null || qrDepositState is null)
            {
                throw new Exception("Deposit Entrypoint or Qr Deposit Not Found");
            }

            var resp = ValidateGlobalTransferPayment(
                depositEntrypoint.TransactionNo!,
                qrDepositState.PaymentReceivedDateTime!.Value,
                qrDepositState.DepositQrGenerateDateTime!.Value,
                qrDepositState.QrCodeExpiredTimeInMinute
            );

            if (resp is null)
            {
                throw new Exception("Validate Global Transfer Payment Failed");
            }

            await context.RespondAsync(resp);
        }
    }

    private GlobalTransferDepositPaymentValidationCompleted? ValidateGlobalTransferPayment(
        string transactionNo,
        DateTime receivedPaymentDataTime,
        DateTime qrCodeGenerateDateTime,
        int expireTimeInMinute
    )
    {
        var expireTime = TimeSpan.FromMinutes(expireTimeInMinute);
        var timeDifference = (receivedPaymentDataTime - qrCodeGenerateDateTime).Duration();

        _logger.LogDebug("Qr payment received for {TransactionNo} in {Hours:h\\:mm} hours", transactionNo, timeDifference);

        if (timeDifference.TotalHours > expireTime.TotalHours)
        {
            var errorMessage = $"This QR code expired. Expect with in {expireTime:h\\:mm} hours but received in {timeDifference:h\\:mm} hours";
            _logger.LogError("TransactionNo: {TransactionNo} Error: {ErrorMessage}", transactionNo, errorMessage);
            throw new QrCodeExpiredException(errorMessage);
        }

        return new GlobalTransferDepositPaymentValidationCompleted(transactionNo);
    }
}