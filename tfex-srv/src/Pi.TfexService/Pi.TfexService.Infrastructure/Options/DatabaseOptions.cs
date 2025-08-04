using System.ComponentModel.DataAnnotations;

namespace Pi.TfexService.Infrastructure.Options;

public class DatabaseOptions
{
    public const string Options = "Database";

    [Required]
    public string AesKey { get; set; } = string.Empty;
}