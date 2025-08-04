using System.ComponentModel.DataAnnotations;

namespace Pi.TfexService.Infrastructure.Options;

public class ItOptions
{
    public const string Options = "It";

    [Required]
    public string Host { get; set; } = string.Empty;

    [Required]
    public Guid? ApiKey { get; set; } = null;
}