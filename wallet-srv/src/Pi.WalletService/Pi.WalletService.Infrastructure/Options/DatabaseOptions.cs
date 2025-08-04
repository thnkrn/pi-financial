using System.ComponentModel.DataAnnotations;

namespace Pi.WalletService.Infrastructure.Options;

public class DatabaseOptions
{
    public const string Options = "Database";

    [Required]
    public string AesKey { get; set; } = string.Empty;
}