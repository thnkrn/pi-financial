namespace Pi.WalletService.Application.Services.Notification;

public interface IEmailNotificationService
{
    Task SendEmail(string userId,
        string customerCode,
        List<string> recipients,
        long templateId,
        List<string>? titlePayload,
        List<string>? bodyPayload,
        CancellationToken cancellationToken);
}