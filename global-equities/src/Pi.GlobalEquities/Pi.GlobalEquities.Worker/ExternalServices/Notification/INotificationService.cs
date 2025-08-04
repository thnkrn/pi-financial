using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Worker.ExternalServices.Notification;

public interface INotificationService
{
    Task SendNotification(NotificationTemplate cmsTemplateId, IOrder order, CancellationToken ct);
    NotificationTemplate? GetCmsTemplateId(OrderStatus orderStatus, OrderSide orderSide);
}
