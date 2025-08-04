using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.Notification;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.SendNotification;

public record DepositWithdrawSuccessNotification(Guid CorrelationId);
public record DepositWithdrawSuccessNotificationResponse(Guid CorrelationId);

public class NotificationSuccessConsumer : IConsumer<DepositWithdrawSuccessNotification>
{
    private readonly ILogger<NotificationSuccessConsumer> _logger;
    private readonly ITransactionQueriesV2 _transactionQueriesV2;
    private readonly INotificationService _notificationService;

    public NotificationSuccessConsumer(
        ILogger<NotificationSuccessConsumer> logger,
        ITransactionQueriesV2 transactionQueriesV2,
        INotificationService notificationService
    )
    {
        _logger = logger;
        _transactionQueriesV2 = transactionQueriesV2;
        _notificationService = notificationService;
    }

    public async Task Consume(ConsumeContext<DepositWithdrawSuccessNotification> context)
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

        await context.RespondAsync(new DepositWithdrawSuccessNotificationResponse(transaction.CorrelationId));
    }

    public long GetTemplateId(TransactionType transactionType, Product product)
    {
        if (transactionType == TransactionType.Deposit)
        {
            return product switch
            {
                Product.CashBalance => 26,
                Product.CreditBalanceSbl => 27,
                Product.Cash => 28,
                Product.Derivatives => 29,
                Product.GlobalEquities => 48,
                _ => throw new ArgumentOutOfRangeException(
                    $"Product: {product.ToString()} Not Support Send Notification")
            };
        }
        else if (transactionType == TransactionType.Withdraw)
        {
            return product == Product.Derivatives ? 24 : 23;
        }
        else
        {
            throw new ArgumentOutOfRangeException(
                $"Transaction: Type {transactionType.ToString()} Not Support Send Notification");
        }
    }
}