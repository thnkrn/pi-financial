namespace Pi.TfexService.Application.Services.Notification;

public interface INotificationService
{
    Task SendNotification(
        string userId,
        string customerCode,
        long templateId,
        List<string>? titlePayload,
        List<string>? bodyPayload,
        bool isPushed,
        bool shouldStoreDb,
        CancellationToken cancellationToken);
}