using Microsoft.Extensions.Logging;
using Pi.Client.NotificationService.Api;
using Pi.Client.NotificationService.Model;
using Pi.TfexService.Application.Services.Notification;

namespace Pi.TfexService.Infrastructure.Services;

public class NotificationService(INotificationApi notificationApi, ILogger<NotificationService> logger)
    : INotificationService
{
    public async Task SendNotification(
        string userId,
        string customerCode,
        long templateId,
        List<string>? titlePayload,
        List<string>? bodyPayload,
        bool isPushed,
        bool shouldStoreDb,
        CancellationToken cancellationToken)
    {
        try
        {
            await notificationApi.InternalNotificationsPostAsync(
                new NotificationCreateRequest(
                    templateId,
                    userId,
                    NotificationType.Order.ToString(),
                    isPushed,
                    shouldStoreDb,
                    titlePayload!,
                    bodyPayload!
                ),
                cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "NotificationService: Unable to send notification for customer: {CustomerCode} with Exception {Exception}",
                customerCode,
                e.Message);
        }
    }
}