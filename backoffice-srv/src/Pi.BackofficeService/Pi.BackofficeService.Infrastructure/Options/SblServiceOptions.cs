using System.ComponentModel.DataAnnotations;

namespace Pi.BackofficeService.Infrastructure.Options;

public class SblServiceOptions
{
    public const string Options = "Sbl";

    [Required] public required string InstrumentBucket { get; set; }
}
