namespace Pi.SetService.Application.Models.Notification;

public record NotificationPayload
{
    public required NotificationTemplate Template { get; init; }
    public required Guid UserId { get; init; }
    public required NotificationType Type { get; init; }
    public required bool IsPush { get; init; }
    public required bool StoreDb { get; init; }
    public required IEnumerable<string> Body { get; init; }
    public IEnumerable<string>? Title { get; init; }
    public string? Url { get; init; }
    public IEnumerable<NotificationTag>? Tags { get; init; }
}
