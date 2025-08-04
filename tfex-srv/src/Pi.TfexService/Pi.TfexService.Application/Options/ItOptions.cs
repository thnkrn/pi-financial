using System.ComponentModel.DataAnnotations;

namespace Pi.TfexService.Application.Options;

public class ItOptions
{
    public const string Options = "It";

    [Required]
    public bool GetDataFromIt { get; set; }
}