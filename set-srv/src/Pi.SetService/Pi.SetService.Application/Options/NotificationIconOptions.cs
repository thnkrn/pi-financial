namespace Pi.SetService.Application.Options;

public class NotificationIconOptions
{
    public const string Options = "NotificationIcon";
    public required string S3IconBaseUrl { get; init; }
}
