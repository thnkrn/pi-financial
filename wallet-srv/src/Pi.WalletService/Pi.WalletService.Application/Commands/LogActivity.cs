using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.Application.Commands;


public record LogActivity(ActivityLogSnapshot Snapshot);

public record LogActivitySuccess(Guid CorrelationId);

public record ActivityLogSnapshot(
    Guid CorrelationId,
    string? TransactionNo,
    TransactionType? TransactionType,
    string? UserId,
    string? AccountCode,
    string? CustomerCode,
    Channel? Channel,
    Product? Product,
    Purpose? Purpose,
    string StateMachine,
    string? State,
    decimal? RequestedAmount,
    DateTime? PaymentReceivedDateTime,
    decimal? PaymentReceivedAmount,
    DateTime? PaymentDisbursedDateTime,
    decimal? PaymentDisbursedAmount,
    decimal? PaymentConfirmedAmount,
    string? OtpRequestRef,
    string? OtpRequestId,
    DateTime? OtpConfirmedDateTime,
    decimal? Fee,
    string? CustomerName,
    string? BankAccountNo,
    string? BankAccountName,
    string? BankAccountTaxId,
    string? BankName,
    string? BankCode,
    string? BankBranchCode,
    DateTime? DepositGeneratedDateTime,
    string? QrTransactionNo,
    string? QrTransactionRef,
    string? QrValue,
    Currency? RequestedCurrency,
    decimal? RequestedFxAmount,
    Currency? RequestedFxCurrency,
    Currency? PaymentReceivedCurrency,
    decimal? TransferFee,
    string? FxTransactionId,
    DateTime? FxInitiateRequestDateTime,
    DateTime? FxConfirmedDateTime,
    decimal? ExchangeAmount,
    Currency? ExchangeCurrency,
    decimal? FxConfirmedExchangeRate,
    decimal? FxConfirmedAmount,
    Currency? FxConfirmedCurrency,
    decimal? FxMarkUp,
    string? TransferFromAccount,
    decimal? TransferAmount,
    string? TransferToAccount,
    Currency? TransferCurrency,
    DateTime? TransferRequestTime,
    DateTime? TransferCompleteTime,
    string? FailedReason,
    string? RequestId,
    string? RequesterDeviceId
);

public class LogActivityConsumer : IConsumer<LogActivity>
{
    private readonly ILogger<LogActivityConsumer> _logger;
    private readonly IActivityLogRepository _activityLogRepository;

    public LogActivityConsumer(
        IActivityLogRepository activityLogRepository,
        ILogger<LogActivityConsumer> logger)
    {
        _activityLogRepository = activityLogRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LogActivity> context)
    {
        _logger.LogInformation("LogActivityConsumer: log from statemachine {StateMachine}", context.Message.Snapshot.StateMachine);
        await LogActivity(context.Message.Snapshot);
        if (context.RequestId != null)
        {
            await context.RespondAsync(new LogActivitySuccess(context.Message.Snapshot.CorrelationId));
        }
    }

    private async Task LogActivity(ActivityLogSnapshot obj)
    {
        try
        {
            _activityLogRepository.Create(
                new ActivityLog(
                    Guid.NewGuid(),
                    obj.CorrelationId,
                    obj.TransactionNo ?? string.Empty,
                    obj.TransactionType ?? TransactionType.Unknown,
                    obj.UserId ?? string.Empty,
                    obj.AccountCode ?? string.Empty,
                    obj.CustomerCode ?? string.Empty,
                    obj.Channel ?? Channel.Unknown,
                    obj.Product ?? Product.Unknown,
                    obj.Purpose ?? Purpose.Unknown,
                    obj.StateMachine,
                    obj.State ?? string.Empty,
                    obj.RequestedAmount ?? 0,
                    obj.PaymentReceivedDateTime ?? null,
                    obj.PaymentReceivedAmount ?? null,
                    obj.PaymentDisbursedDateTime ?? null,
                    obj.PaymentDisbursedAmount ?? null,
                    obj.PaymentConfirmedAmount ?? null,
                    obj.OtpRequestRef ?? null,
                    obj.OtpRequestId ?? null,
                    obj.OtpConfirmedDateTime ?? null,
                    obj.Fee ?? null,
                    obj.CustomerName ?? null,
                    obj.BankAccountNo ?? null,
                    obj.BankAccountName ?? null,
                    obj.BankAccountTaxId ?? null,
                    obj.BankName ?? null,
                    obj.BankCode ?? null,
                    obj.BankBranchCode ?? null,
                    obj.DepositGeneratedDateTime ?? null,
                    obj.QrTransactionNo ?? null,
                    obj.QrTransactionRef ?? null,
                    obj.QrValue ?? null,
                    obj.RequestedCurrency ?? null,
                    obj is { RequestedAmount: not null, RequestedCurrency: not null }
                        ? $"{obj.RequestedAmount} {obj.RequestedCurrency}"
                        : null,
                    obj.RequestedFxAmount ?? null,
                    obj.RequestedFxCurrency ?? null,
                    obj is { RequestedFxAmount: not null, RequestedFxCurrency: not null }
                        ? $"{obj.RequestedFxAmount} {obj.RequestedFxCurrency}"
                        : null,
                    obj.PaymentReceivedCurrency ?? null,
                    obj.TransferFee ?? null,
                    obj.FxTransactionId ?? null,
                    obj.ExchangeAmount ?? null,
                    obj.ExchangeCurrency ?? null,
                    obj.FxInitiateRequestDateTime ?? null,
                    obj.FxConfirmedDateTime ?? null,
                    obj.FxConfirmedExchangeRate ?? null,
                    obj.FxConfirmedAmount ?? null,
                    obj.FxConfirmedCurrency ?? null,
                    obj is { FxConfirmedAmount: not null, FxConfirmedCurrency: not null }
                        ? $"{obj.FxConfirmedAmount} {obj.FxConfirmedCurrency}"
                        : null,
                    obj.TransferFromAccount ?? null,
                    obj.TransferAmount ?? null,
                    obj.TransferToAccount ?? null,
                    obj.TransferCurrency ?? null,
                    obj is { TransferAmount: not null, TransferCurrency: not null }
                        ? $"{obj.TransferAmount} {obj.TransferCurrency}"
                        : null,
                    obj.TransferRequestTime ?? null,
                    obj.TransferCompleteTime ?? null,
                    obj.FailedReason ?? null,
                    obj.RequestId ?? null,
                    obj.RequesterDeviceId ?? null
                    )
            );

            await _activityLogRepository.UnitOfWork.SaveChangesAsync();

        }
        catch (Exception e)
        {
            _logger.LogError(e, "LogActivity: Unable to store transaction with Exception: {Message}", e.Message);
        }
    }
}
