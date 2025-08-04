using Pi.SetService.Application.Models.Notification;

namespace Pi.SetService.Application.Services.NotificationService;

public interface INotificationService
{
    Task<Guid?> SendNotification(NotificationPayload payload);
}
