using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.Notification;
using Pi.TfexService.Application.Utils;

namespace Pi.TfexService.Application.Commands.Notification;

public class NotificationConsumer(INotificationService notificationService, ILogger<NotificationConsumer> logger) : IConsumer<SendNotificationRequest>
{
    public async Task Consume(ConsumeContext<SendNotificationRequest> context)
    {
        try
        {
            var order = context.Message.Order;
            var templateId = GetTemplateId(order);
            if (templateId == 0)
            {
                throw new ArgumentException("Notification Template not matched");
            }

            var payload = CreatePayload(order);

            await notificationService.SendNotification(
                context.Message.UserId,
                context.Message.CustomerCode,
                templateId,
                new List<string>(),
                payload,
                true,
                true,
                context.CancellationToken
            );

            await context.RespondAsync(new SendNotificationSuccess(order.OrderNo, order.Status));
        }
        catch (Exception e)
        {
            logger.LogError(e, "NotificationConsumer: Unable to notify {Customer} with Exception {Exception}", context.Message.CustomerCode, e.Message);
        }

    }

    private List<string> CreatePayload(SetTradeOrderStatus order)
    {
        var payload = new List<string>();
        if (string.IsNullOrWhiteSpace(order.SeriesId))
        {
            throw new ArgumentException("Symbol is missing");
        }

        // payload consists of {order No} {symbol} {units} {price} {Qty - Optional}
        payload.Add(order.OrderNo);
        payload.Add(order.SeriesId);
        payload.Add(order.Volume.ToString("N0"));
        if (order.Price != 0) payload.Add(ConvertUtils.ConvertDecimalWithoutRounding(order.Price, 2));

        switch (order.Status)
        {
            case "MP":
                payload.Add(order.MatchedVolume.ToString("N0"));
                break;
            case "CP" or "EP":
                payload.Add(order.CancelledVolume.ToString("N0"));
                break;
        }

        return payload;
    }

    private static long GetTemplateId(SetTradeOrderStatus order)
    {
        var hasPrice = order.Price != 0;
        return (order.Status, order.Side, hasPrice) switch
        {
            // Order Matched
            ("M", SetTradeListenerOrderEnum.Side.Long, true) => 50,
            ("M", SetTradeListenerOrderEnum.Side.Short, true) => 51,
            ("M", SetTradeListenerOrderEnum.Side.Long, false) => 82,
            ("M", SetTradeListenerOrderEnum.Side.Short, false) => 83,

            // Order Rejected
            ("RS", SetTradeListenerOrderEnum.Side.Long, true) => 52,
            ("RS", SetTradeListenerOrderEnum.Side.Short, true) => 53,
            ("RS", SetTradeListenerOrderEnum.Side.Long, false) => 84,
            ("RS", SetTradeListenerOrderEnum.Side.Short, false) => 85,

            // Order Partially Matched
            ("MP", SetTradeListenerOrderEnum.Side.Long, true) => 54,
            ("MP", SetTradeListenerOrderEnum.Side.Short, true) => 56,
            ("MP", SetTradeListenerOrderEnum.Side.Long, false) => 86,
            ("MP", SetTradeListenerOrderEnum.Side.Short, false) => 88,

            // Order Partially Cancelled
            ("CP", SetTradeListenerOrderEnum.Side.Long, true) => 55,
            ("CP", SetTradeListenerOrderEnum.Side.Short, true) => 57,
            ("CP", SetTradeListenerOrderEnum.Side.Long, false) => 87,
            ("CP", SetTradeListenerOrderEnum.Side.Short, false) => 89,

            // Order Cancelled
            ("CX", SetTradeListenerOrderEnum.Side.Long, true) => 58,
            ("CX", SetTradeListenerOrderEnum.Side.Short, true) => 59,
            ("CX", SetTradeListenerOrderEnum.Side.Long, false) => 90,
            ("CX", SetTradeListenerOrderEnum.Side.Short, false) => 91,

            // Order Partially Expired
            ("EP", SetTradeListenerOrderEnum.Side.Long, true) => 94,
            ("EP", SetTradeListenerOrderEnum.Side.Short, true) => 95,
            ("EP", SetTradeListenerOrderEnum.Side.Long, false) => 96,
            ("EP", SetTradeListenerOrderEnum.Side.Short, false) => 97,

            // Order Expired
            ("E", SetTradeListenerOrderEnum.Side.Long, true) => 92,
            ("E", SetTradeListenerOrderEnum.Side.Short, true) => 93,
            ("E", SetTradeListenerOrderEnum.Side.Long, false) => 98,
            ("E", SetTradeListenerOrderEnum.Side.Short, false) => 99,

            // Not supported status
            _ => 0
        };
    }
}