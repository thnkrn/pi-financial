using Microsoft.Extensions.Logging;
using Pi.Client.NotificationService.Api;
using Pi.Client.NotificationService.Model;
using Pi.WalletService.Application.Services.Notification;

namespace Pi.WalletService.Infrastructure.Services;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly IEmailApi _emailApi;
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(IEmailApi emailApi, ILogger<EmailNotificationService> logger)
    {
        _emailApi = emailApi;
        _logger = logger;
    }

    public async Task SendEmail(
        string userId,
        string customerCode,
        List<string> recipients,
        long templateId,
        List<string>? titlePayload,
        List<string>? bodyPayload,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _emailApi.InternalEmailPostAsync(
                new EmailRequestDto(
                    userId,
                    customerCode,
                    recipients,
                    templateId,
                    Language.Thai,
                    titlePayload ?? new List<string>(),
                    bodyPayload ?? new List<string>()
                ),
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Unable to send email for customer: {CustomerCode} with Exception {Exception}",
                customerCode,
                e.Message);
        }
    }
}