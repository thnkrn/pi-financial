using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Common.Features;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.GlobalWalletTransfer;

public record PrepareGlobalTransferFxExchangeData(Guid CorrelationId, decimal RequestedFxRate, decimal MarkupRate);

public record PrepareGlobalTransferFxExchangeDataSuccess(Guid CorrelationId);

public class PrepareGlobalTransferFxExchangeDataConsumer :
    SagaConsumer,
    IConsumer<PrepareGlobalTransferFxExchangeData>
{
    private readonly IQrDepositRepository _qrDepositRepository;
    private readonly IOddDepositRepository _oddDepositRepository;
    private readonly IAtsDepositRepository _atsDepositRepository;
    private readonly IGlobalTransferRepository _globalTransferRepository;
    private readonly IFeatureService _featureService;
    private readonly ILogger<PrepareGlobalTransferFxExchangeData> _logger;

    public PrepareGlobalTransferFxExchangeDataConsumer(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        IQrDepositRepository qrDepositRepository,
        IOddDepositRepository oddDepositRepository,
        IGlobalTransferRepository globalTransferRepository,
        IAtsDepositRepository atsDepositRepository,
        ILogger<PrepareGlobalTransferFxExchangeData> logger, IFeatureService featureService) : base(depositEntrypointRepository,
        withdrawEntrypointRepository)
    {
        _qrDepositRepository = qrDepositRepository;
        _oddDepositRepository = oddDepositRepository;
        _globalTransferRepository = globalTransferRepository;
        _atsDepositRepository = atsDepositRepository;
        _logger = logger;
        _featureService = featureService;
    }

    public async Task Consume(ConsumeContext<PrepareGlobalTransferFxExchangeData> context)
    {
        try
        {
            var transaction = await GetDepositEntrypointByIdAsync(context.Message.CorrelationId);

            if (transaction == null)
            {
                throw new InvalidDataException(
                    $"depositEntrypointState not found, CorrelationId: {context.Message.CorrelationId}");
            }

            decimal paymentReceivedAmount;
            decimal fee;
            switch (transaction.Channel)
            {
                case Channel.QR:
                    {
                        var qrDepositState =
                            await _qrDepositRepository.GetAsync(ctx => ctx.CorrelationId == context.Message.CorrelationId);

                        if (qrDepositState is null)
                        {
                            throw new InvalidDataException($"QrDepositState not found, CorrelationId: {context.Message.CorrelationId}");
                        }

                        paymentReceivedAmount = qrDepositState.PaymentReceivedAmount!.Value;
                        fee = qrDepositState.Fee ?? 0;
                        break;
                    }
                case Channel.ATS:
                    {
                        var atsDepositState = await _atsDepositRepository.GetAsync(ctx => ctx.CorrelationId == context.Message.CorrelationId);

                        if (atsDepositState is null)
                        {
                            throw new InvalidDataException($"AtsDepositState not found, CorrelationId: {context.Message.CorrelationId}");
                        }

                        paymentReceivedAmount = atsDepositState.PaymentReceivedAmount!.Value;
                        fee = atsDepositState.Fee ?? 0;
                        break;
                    }
                case Channel.ODD:
                    {
                        var oddDepositState =
                            await _oddDepositRepository.GetAsync(ctx => ctx.CorrelationId == context.Message.CorrelationId);

                        if (oddDepositState is null)
                        {
                            throw new InvalidDataException($"OddDepositState not found, CorrelationId: {context.Message.CorrelationId}");
                        }

                        paymentReceivedAmount = oddDepositState.PaymentReceivedAmount!.Value;
                        fee = oddDepositState.Fee ?? 0;
                        break;
                    }
                case Channel.SetTrade:
                case Channel.OnlineViaKKP:
                case Channel.EForm:
                case Channel.TransferApp:
                case Channel.Unknown:
                default:
                    throw new InvalidDataException($"Channel {transaction.Channel} not supported.");
            }

            var exchangeCurrency = Currency.THB;
            var exchangeAmount = paymentReceivedAmount - fee;

            if (context.Message.MarkupRate > 0)
            {
                exchangeCurrency = Currency.USD;
                exchangeAmount = RoundingUtils.RoundExchangeTransaction(
                    TransactionType.Deposit,
                    Currency.THB,
                    paymentReceivedAmount - fee,
                    exchangeCurrency,
                    context.Message.RequestedFxRate);
            }

            await _globalTransferRepository.UpdateExchangeData(
                context.Message.CorrelationId,
                exchangeAmount,
                exchangeCurrency
            );

            await context.RespondAsync(
                new PrepareGlobalTransferFxExchangeDataSuccess(context.Message.CorrelationId));
        }
        catch (Exception e)
        {
            _logger.LogError("PrepareGlobalTransferPaymentReceivedData error {exception}", e.Message);
            throw new Exception("UpdateGlobalTransferData error", e);
        }
    }
}