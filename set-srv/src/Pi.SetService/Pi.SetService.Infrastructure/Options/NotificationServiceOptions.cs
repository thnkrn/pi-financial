using System.ComponentModel.DataAnnotations;

namespace Pi.SetService.Infrastructure.Options;

public class NotificationServiceOptions
{
    public const string Options = "Notification";

    [Required]
    public string Host { get; set; } = string.Empty;
    public int TimeoutMS { get; set; }
    public short MaxRetry { get; set; }
}
