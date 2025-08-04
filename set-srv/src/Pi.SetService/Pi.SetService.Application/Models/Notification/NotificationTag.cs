namespace Pi.SetService.Application.Models.Notification;

public record NotificationTag
{
    public required string Payload { get; init; }
    public required string TextColor { get; init; }
    public required NotificationTagType Type { get; init; }
    public string? Icon { get; init; }
    public string? BackgroundColor { get; init; }
}
