using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.Notification;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.SendNotification;

public record DepositWithdrawFailedNotification(Guid CorrelationId);
public record DepositWithdrawFailedNotificationResponse(Guid CorrelationId);

public class NotificationFailedConsumer : IConsumer<DepositWithdrawFailedNotification>
{
    private readonly ILogger<NotificationFailedConsumer> _logger;
    private readonly ITransactionQueriesV2 _transactionQueriesV2;
    private readonly INotificationService _notificationService;

    public NotificationFailedConsumer(
        ILogger<NotificationFailedConsumer> logger,
        ITransactionQueriesV2 transactionQueriesV2,
        INotificationService notificationService
        )
    {
        _logger = logger;
        _transactionQueriesV2 = transactionQueriesV2;
        _notificationService = notificationService;
    }

    public async Task Consume(ConsumeContext<DepositWithdrawFailedNotification> context)
    {
        var transaction = await _transactionQueriesV2.GetTransactionById(context.Message.CorrelationId);
        if (transaction == null || string.IsNullOrEmpty(transaction.TransactionNo))
        {
            throw new InvalidDataException("Transaction Not Found");
        }

        var templateId = GetTemplateId(transaction.TransactionType, transaction.Product);
        var bodyPayload = new List<string>();
        if (transaction.Product == Product.GlobalEquities)
        {
            bodyPayload.Add(transaction.GetTransferAmount()!.Value.ToString("0.00"));
            bodyPayload.Add(transaction.GetRequestedFxCurrency()?.ToString() ?? Currency.USD.ToString());
        }
        else
        {
            bodyPayload.Add(transaction.RequestedAmount.ToString("N", CultureInfo.InvariantCulture));
            bodyPayload.Add(Currency.THB.ToString());
        }

        await _notificationService.SendNotification(
            transaction.UserId,
            transaction.CustomerCode,
            templateId,
            new List<string>(),
            bodyPayload,
            true,
            true,
            context.CancellationToken);

        await context.RespondAsync(new DepositWithdrawFailedNotificationResponse(transaction.CorrelationId));
    }

    public long GetTemplateId(TransactionType transactionType, Product product)
    {
        if (transactionType == TransactionType.Deposit)
        {
            return product switch
            {
                Product.CashBalance => 30,
                Product.CreditBalanceSbl => 31,
                Product.Cash => 32,
                Product.Derivatives => 33,
                Product.GlobalEquities => 49,
                _ => throw new ArgumentOutOfRangeException(
                    $"Product: {product.ToString()} Not Support Send Notification")
            };
        }
        else if (transactionType == TransactionType.Withdraw)
        {
            return 25;
        }
        else
        {
            throw new ArgumentOutOfRangeException(
                $"Transaction: Type {transactionType.ToString()} Not Support Send Notification");
        }
    }
}