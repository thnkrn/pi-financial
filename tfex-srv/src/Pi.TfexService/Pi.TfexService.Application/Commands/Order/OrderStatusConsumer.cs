using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi.Common.Features;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.DistributedLock;
using Pi.TfexService.Application.Services.UserService;

namespace Pi.TfexService.Application.Commands.Order;

public class OrderStatusConsumer(
    IFeatureService featureService,
    IUserService userService,
    IUserV2Service userV2Service,
    IDistributedCache cache,
    IDistributedLockService distributedLockService,
    IBus bus,
    ILogger<OrderStatusConsumer> logger)
    : IConsumer<SetTradeOrderStatus>
{
    private readonly HashSet<string> _supportedStatus = ["M", "MP", "RS", "C", "CX", "CP", "E", "EP"];
    public async Task Consume(ConsumeContext<SetTradeOrderStatus> context)
    {
        try
        {
            if (featureService.IsOff(Features.TfexListenerNotification)) return;
            if (!_supportedStatus.Contains(context.Message.Status)) return;
            if (string.IsNullOrEmpty(context.Message.OrderNo) || string.IsNullOrEmpty(context.Message.Status)) return;

            var eventName = $"{context.Message.OrderNo}:{context.Message.Status}";
            if (await distributedLockService.AddEventAsync(eventName))
            {
                var (userId, customerCode) = await GetUserCredential(context.Message.AccountNo);

                await HandleSendNotification(context, userId, customerCode);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "OrderStatusConsumer: {Message}", e.Message);
        }
    }

    private async Task<(string userId, string customerCode)> GetUserCredential(string accountCode)
    {
        if (string.IsNullOrWhiteSpace(accountCode) || accountCode.Length < 8)
        {
            throw new ArgumentException($"Invalid AccountCode: {accountCode}", nameof(accountCode));
        }

        var customerCode = accountCode[..^1];
        var user = featureService.IsOn(Features.MigrateUserV2)
            ? await userV2Service.GetUserByCustomerCode(customerCode)
            : await userService.GetUserByCustomerCode(customerCode);

        return (user.Id.ToString(), customerCode);
    }

    private async Task HandleSendNotification(ConsumeContext<SetTradeOrderStatus> context, string userId,
        string customerCode)
    {
        // Handle false partial match notification
        var message = context.Message;
        if (message.Status == "MP")
        {
            var key = $"{message.OrderNo}:{message.Status}";
            var previousMessage = await cache.GetStringAsync(key);
            SetTradeOrderStatus? previousSetTradeOrderStatus = null;

            if (previousMessage != null)
            {
                previousSetTradeOrderStatus = JsonSerializer.Deserialize<SetTradeOrderStatus>(previousMessage);
            }

            if (previousMessage == null ||
                (previousSetTradeOrderStatus != null && previousSetTradeOrderStatus.MatchedVolume != message.MatchedVolume))
            {
                var cacheEntryOptions =
                    new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));
                await cache.SetStringAsync(key, JsonSerializer.Serialize(message), cacheEntryOptions);
                await bus.Publish(new SendNotificationRequest(userId, customerCode, message));
            }
        }
        else
        {
            await bus.Publish(new SendNotificationRequest(userId, customerCode, message));
        }
    }
}