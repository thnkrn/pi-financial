using Pi.Client.NotificationService.Api;
using Pi.Client.NotificationService.Model;
using Pi.SetService.Application.Models.Notification;
using Pi.SetService.Application.Services.NotificationService;
using NotificationTagType = Pi.SetService.Application.Models.Notification.NotificationTagType;

namespace Pi.SetService.Infrastructure.Services;

public class NotificationService(INotificationApi notificationApi) : INotificationService
{
    public async Task<Guid?> SendNotification(NotificationPayload payload)
    {
        var request = new NotificationCreateRequest(
                (long)payload.Template,
                payload.UserId.ToString(),
                payload.Type.ToString(),
                payload.IsPush,
                payload.StoreDb
            );
        request.PayloadBody = payload.Body.ToList();

        if (payload.Title != null) request.PayloadTitle = payload.Title.ToList();
        if (payload.Url != null) request.Url = payload.Url;
        if (payload.Tags != null)
            request.Tags = payload.Tags.Select(q =>
            {
                var type = q.Type switch
                {
                    NotificationTagType.TextBadge => Client.NotificationService.Model.NotificationTagType.TextBadge,
                    NotificationTagType.LocalizedTextBadge => Client.NotificationService.Model.NotificationTagType
                        .LocalizedTextBadge,
                    NotificationTagType.InstrumentBadge => Client.NotificationService.Model.NotificationTagType
                        .InstrumentBadge,
                    NotificationTagType.IconBadge => Client.NotificationService.Model.NotificationTagType.IconBadge,
                    _ => throw new ArgumentOutOfRangeException(nameof(q.Type), q.Type, null)
                };

                return new NotificationTagCreateRequest(q.Icon ?? "", q.Payload, q.BackgroundColor ?? "", q.TextColor,
                    type);
            }).ToList();

        var response = await notificationApi.InternalNotificationsPostAsync(request);

        return response.Data.TicketId;
    }
}
