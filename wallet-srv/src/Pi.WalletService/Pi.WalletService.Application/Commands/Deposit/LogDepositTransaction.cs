using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Deposit;

public record LogDepositTransaction(DepositTransactionSnapshot Snapshot);

public record LogDepositTransactionSuccess(string TransactionNo);

public record DepositTransactionSnapshot(
    Guid CorrelationId,
    string UserId,
    string CurrentState,
    string CustomerCode,
    string TransactionNo,
    decimal RequestedAmount,
    string AccountCode,
    Channel Channel,
    Product Product,
    Purpose Purpose,
    decimal? BankFee,
    DateTime? DepositQrGenerateDateTime,
    string? QrTransactionNo,
    string? QrTransactionRef,
    string? QrValue,
    DateTime? PaymentReceivedDateTime,
    decimal? PaymentReceivedAmount,
    string? CustomerName,
    string? BankAccountName,
    string? BankName,
    string? BankCode,
    string? BankAccountNo,
    string? FailedReason,
    Guid? RequesterDeviceId
);



public class LogDepositTransactionConsumer : IConsumer<LogDepositTransaction>
{
    private readonly ILogger<LogDepositTransactionConsumer> _logger;
    private readonly ITransactionHistoryRepository _transactionHistoryRepository;

    public LogDepositTransactionConsumer(
        ITransactionHistoryRepository transactionHistoryRepository,
        ILogger<LogDepositTransactionConsumer> logger)
    {
        _transactionHistoryRepository = transactionHistoryRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LogDepositTransaction> context)
    {
        await Log(context.Message.Snapshot);
        if (context.RequestId != null)
        {
            await context.RespondAsync(new LogDepositTransactionSuccess(context.Message.Snapshot.TransactionNo));
        }
    }

    private async Task Log(DepositTransactionSnapshot obj)
    {
        try
        {
            _transactionHistoryRepository.Create(
                new TransactionHistory(
                    Guid.NewGuid(),
                    obj.CorrelationId,
                    obj.TransactionNo,
                    TransactionType.Deposit,
                    obj.UserId,
                    obj.AccountCode,
                    obj.CustomerCode,
                    string.Empty,
                    obj.Channel,
                    obj.Product,
                    obj.Purpose,
                    obj.CurrentState,
                    obj.RequestedAmount,
                    obj.BankFee,
                    obj.PaymentReceivedDateTime,
                    obj.PaymentReceivedAmount,
                    obj.CustomerName,
                    obj.BankAccountName,
                    obj.BankName,
                    obj.BankCode,
                    obj.BankAccountNo,
                    obj.FailedReason,
                    obj.RequesterDeviceId
                ));

            await _transactionHistoryRepository.UnitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "LogTransaction: Unable to store transaction with Exception: {Message}", e.Message);
        }
    }
}
