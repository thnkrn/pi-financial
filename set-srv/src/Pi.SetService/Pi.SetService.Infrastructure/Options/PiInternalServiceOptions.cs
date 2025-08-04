using System.ComponentModel.DataAnnotations;

namespace Pi.SetService.Infrastructure.Options;

public class PiInternalServiceOptions
{
    public const string Options = "PiInternal";
    [Required] public string ApiKey { get; set; } = string.Empty;
    [Required] public string Host { get; set; } = string.Empty;

    public int TimeoutMS { get; set; }

    public short MaxRetry { get; set; }
}