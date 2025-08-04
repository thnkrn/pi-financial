using System.ComponentModel.DataAnnotations;

namespace Pi.TfexService.Application.Options;

public class SetTradeOptions
{
    public const string Options = "SetTrade";

    [Required]
    public bool UsePlByCost { get; set; }
}