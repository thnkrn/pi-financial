using System.Globalization;
using Pi.Client.NotificationService.Api;
using Pi.Client.NotificationService.Model;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Pi.GlobalEquities.Worker.ExternalServices.FeatureFlags;

namespace Pi.GlobalEquities.Worker.ExternalServices.Notification;

public class NotificationService : INotificationService
{
    private readonly INotificationApi _notificationApi;
    private ILogger<NotificationService> _logger;
    public NotificationService(INotificationApi notificationApi, ILogger<NotificationService> logger)
    {
        _notificationApi = notificationApi;
        _logger = logger;
    }

    public async Task SendNotification(NotificationTemplate cmsTemplateId, IOrder order, CancellationToken ct)
    {
        try
        {
            var notificationPayload = new NotificationPayload
            {
                TemplateNo = cmsTemplateId,
                Order = order
            };
            var notificationRequest = new NotificationCreateRequest
            (
                cmsTemplateId: (int)notificationPayload.TemplateNo,
                userId: notificationPayload.Order.UserId,
                type: "order",
                isPushed: true,
                shouldStoreDb: true,
                payloadTitle: new List<string>(),
                payloadBody: notificationPayload.Payload.ToList()
            );

            await _notificationApi.InternalNotificationsPostAsync(notificationRequest, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on Send Notification OrderId: {OrderId}, UserId: {UserId}.", order.Id,
                order.UserId);
        }
    }

    public NotificationTemplate? GetCmsTemplateId(OrderStatus orderStatus, OrderSide orderSide)
    {
        NotificationTemplate? cmsTemplateId = orderStatus switch
        {
            OrderStatus.Matched => orderSide == OrderSide.Buy ? NotificationTemplate.BuyMatched : NotificationTemplate.SellMatched,
            OrderStatus.Cancelled => orderSide == OrderSide.Buy ? NotificationTemplate.BuyCancelled : NotificationTemplate.SellCancelled,
            OrderStatus.Rejected => orderSide == OrderSide.Buy ? NotificationTemplate.BuyRejected : NotificationTemplate.SellRejected,
            OrderStatus.PartiallyMatched => orderSide == OrderSide.Buy ? NotificationTemplate.BuyOrderPartiallyMatched : NotificationTemplate.SellOrderPartiallyMatched,
            OrderStatus.Processing or OrderStatus.Queued => null,
            _ => throw new ArgumentOutOfRangeException(nameof(orderStatus), orderStatus, $"Notification template of {orderStatus} could not be found.")
        };
        return cmsTemplateId;
    }
}
