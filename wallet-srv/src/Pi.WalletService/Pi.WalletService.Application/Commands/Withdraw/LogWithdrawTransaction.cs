using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Withdraw;

public record LogWithdrawTransaction(WithdrawTransactionSnapshot Snapshot);

public record LogWithdrawTransactionSuccess(string TransactionNo);

public record WithdrawTransactionSnapshot(
    Guid CorrelationId,
    string UserId,
    string CurrentState,
    string CustomerCode,
    string TransactionNo,
    string AccountCode,
    Channel Channel,
    Product Product,
    decimal? BankFee,
    DateTime? PaymentDisbursedDateTime,
    decimal? PaymentDisbursedAmount,
    string? BankName,
    string? BankCode,
    string? BankAccountNo,
    string? FailedReason,
    Guid? RequesterDeviceId
);

public class LogWithdrawTransactionConsumer : IConsumer<LogWithdrawTransaction>
{
    private readonly ILogger<LogWithdrawTransactionConsumer> _logger;
    private readonly ITransactionHistoryRepository _transactionHistoryRepository;

    public LogWithdrawTransactionConsumer(ITransactionHistoryRepository transactionHistoryRepository, ILogger<LogWithdrawTransactionConsumer> logger)
    {
        _transactionHistoryRepository = transactionHistoryRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LogWithdrawTransaction> context)
    {
        await Log(context.Message.Snapshot);
        if (context.RequestId != null)
        {
            await context.RespondAsync(new LogWithdrawTransactionSuccess(context.Message.Snapshot.TransactionNo));
        }
    }

    private async Task Log(WithdrawTransactionSnapshot obj)
    {
        try
        {
            _transactionHistoryRepository.Create(
                new TransactionHistory(
                    Guid.NewGuid(),
                    obj.CorrelationId,
                    obj.TransactionNo,
                    TransactionType.Withdraw,
                    obj.UserId,
                    obj.AccountCode,
                    obj.CustomerCode,
                    string.Empty,
                    obj.Channel,
                    obj.Product,
                    Purpose.Withdraw,
                    obj.CurrentState,
                    0,
                    obj.BankFee,
                    obj.PaymentDisbursedDateTime,
                    obj.PaymentDisbursedAmount,
                    null,
                    null,
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