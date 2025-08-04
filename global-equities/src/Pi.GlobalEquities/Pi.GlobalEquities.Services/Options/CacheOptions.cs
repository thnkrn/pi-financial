using System.ComponentModel.DataAnnotations;

namespace Pi.GlobalEquities.Services.Options;

public class CacheOptions
{
    public const string Options = "Cache";

    [Required] public int ExchangeRateExpirationMins { get; set; } = 5;
    [Required] public int FallbackExpirationDays { get; set; } = 7;
}
