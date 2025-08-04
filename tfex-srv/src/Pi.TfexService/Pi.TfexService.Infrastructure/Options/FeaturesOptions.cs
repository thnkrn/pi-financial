using System.ComponentModel.DataAnnotations;

namespace Pi.TfexService.Infrastructure.Options;

public class FeaturesOptions
{
    public const string Options = "Features";

    public int TfexNotificationExpireTimeSecond { get; set; }

    public bool IsTfexListenerNotificationEnabled { get; set; }
}