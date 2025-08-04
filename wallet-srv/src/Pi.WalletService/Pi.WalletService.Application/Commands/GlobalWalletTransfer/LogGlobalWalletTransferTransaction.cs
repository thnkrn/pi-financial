using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.GlobalWalletTransfer;

public record LogGlobalWalletTransferTransaction(GlobalWalletTransferTransactionSnapshot Snapshot);

public record LogGlobalWalletTransferTransactionSuccess(string TransactionNo);

public record GlobalWalletTransferTransactionSnapshot(
    string UserId,
    string CurrentState,
    long CustomerId,
    string CustomerCode,
    string GlobalAccount,
    Guid CorrelationId,
    string TransactionNo,
    TransactionType TransactionType,
    decimal RequestedAmount,
    Currency RequestedCurrency,
    decimal RequestedFxAmount,
    Currency RequestedFxCurrency,
    decimal? PaymentReceivedAmount,
    Currency? PaymentReceivedCurrency,
    DateTime? FxInitiateRequestDateTime,
    string? FxTransactionId,
    DateTime? FxConfirmedDateTime,
    decimal? FxConfirmedExchangeRate,
    decimal? FxConfirmedAmount,
    Currency? FxConfirmedCurrency,
    string? TransferFromAccount,
    decimal? TransferAmount,
    string? TransferToAccount,
    Currency? TransferCurrency,
    DateTime? TransferRequestTime,
    DateTime? TransferCompleteTime,
    decimal? TransactionFee,
    decimal? RefundAmount,
    decimal? NetAmount,
    Guid? RequesterDeviceId
);

public class LogGlobalWalletTransferTransactionConsumer :
    IConsumer<LogGlobalWalletTransferTransaction>
{
    private readonly IGlobalWalletTransactionHistoryRepository _globalWalletTransactionHistoryRepository;
    private readonly ILogger<LogGlobalWalletTransferTransactionConsumer> _logger;

    public LogGlobalWalletTransferTransactionConsumer(
        IGlobalWalletTransactionHistoryRepository globalWalletTransactionHistoryRepository,
        ILogger<LogGlobalWalletTransferTransactionConsumer> logger
    )
    {
        _globalWalletTransactionHistoryRepository = globalWalletTransactionHistoryRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LogGlobalWalletTransferTransaction> context)
    {
        await Log(context.Message.Snapshot);
        if (context.RequestId != null)
        {
            await context.RespondAsync(new LogGlobalWalletTransferTransactionSuccess(context.Message.Snapshot.TransactionNo));
        }
    }

    private async Task Log(GlobalWalletTransferTransactionSnapshot obj)
    {
        try
        {
            _globalWalletTransactionHistoryRepository.Create(
                new GlobalWalletTransactionHistory(
                    Guid.NewGuid(),
                    obj.CorrelationId,
                    obj.UserId,
                    obj.CurrentState,
                    obj.CustomerId,
                    obj.CustomerCode,
                    obj.GlobalAccount,
                    obj.TransactionNo,
                    obj.TransactionType,
                    obj.RequestedAmount,
                    obj.RequestedCurrency.ToString(),
                    $"{obj.RequestedAmount} {obj.RequestedCurrency}",
                    obj.RequestedFxAmount,
                    obj.RequestedFxCurrency.ToString(),
                    $"{obj.RequestedFxAmount} {obj.RequestedFxCurrency}",
                    obj.PaymentReceivedAmount,
                    obj.PaymentReceivedCurrency?.ToString(),
                    obj.FxInitiateRequestDateTime,
                    obj.FxTransactionId,
                    obj.FxConfirmedExchangeRate,
                    obj.FxConfirmedDateTime,
                    obj.FxConfirmedAmount,
                    obj.FxConfirmedCurrency.ToString(),
                    obj is { FxConfirmedAmount: not null, FxConfirmedCurrency: not null }
                        ? $"{obj.FxConfirmedAmount} {obj.FxConfirmedCurrency}"
                        : null,
                    obj.TransferFromAccount,
                    obj.TransferAmount,
                    obj.TransferToAccount,
                    obj.TransferCurrency.ToString(),
                    obj is { TransferAmount: not null, TransferCurrency: not null }
                        ? $"{obj.TransferAmount} {obj.TransferCurrency}"
                        : null,
                    obj.TransferRequestTime,
                    obj.TransferCompleteTime,
                    obj.TransactionFee,
                    obj.RefundAmount,
                    obj.NetAmount,
                    obj.RequesterDeviceId
                )
            );

            await _globalWalletTransactionHistoryRepository.UnitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "LogTransaction: Unable to store transaction with Exception: {Message}", e.Message);
        }
    }
}