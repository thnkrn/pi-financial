using Microsoft.Extensions.Logging;
using Pi.Client.NotificationService.Api;
using Pi.Client.NotificationService.Model;
using Pi.WalletService.Application.Services.Notification;

namespace Pi.WalletService.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly INotificationApi _notificationApi;

    public NotificationService(ILogger<NotificationService> logger, INotificationApi notificationApi)
    {
        _logger = logger;
        _notificationApi = notificationApi;
    }

    public async Task SendNotification(
        string userId,
        string customerCode,
        long templateId,
        List<string>? titlePayload,
        List<string>? bodyPayload,
        bool isPushed,
        bool shouldStoreDb,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _notificationApi.InternalNotificationsPostAsync(
                new NotificationCreateRequest(
                    templateId,
                    userId,
                    NotificationType.Wallet.ToString(),
                    isPushed,
                    shouldStoreDb,
                    titlePayload!,
                    bodyPayload!
                ),
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Unable to send notification for customer: {CustomerCode} with Exception {Exception}",
                customerCode,
                e.Message);
        }
    }
}