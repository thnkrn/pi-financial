using System.ComponentModel.DataAnnotations;

namespace Pi.TfexService.Infrastructure.Options;

public class OperationHoursOptions
{
    public const string Options = "OperationHours";

    [Required]
    public string Start { get; set; } = string.Empty;

    [Required]
    public string End { get; set; } = string.Empty;
}