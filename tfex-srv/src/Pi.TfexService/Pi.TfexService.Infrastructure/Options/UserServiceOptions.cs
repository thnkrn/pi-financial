using System.ComponentModel.DataAnnotations;

namespace Pi.TfexService.Infrastructure.Options;

public class UserServiceOptions
{
    public const string Options = "UserService";

    [Required]
    public string Host { get; set; } = string.Empty;

    public int Timeout { get; set; }

    public short MaxRetry { get; set; }
}