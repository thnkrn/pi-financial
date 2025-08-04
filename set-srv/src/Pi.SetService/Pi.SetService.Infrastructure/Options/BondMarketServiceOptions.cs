using System.ComponentModel.DataAnnotations;

namespace Pi.SetService.Infrastructure.Options
{
    public class BondMarketServiceOptions
    {
        public const string Options = "Bond";

        [Required]
        public string Host { get; set; } = string.Empty;

        public int TimeoutMS { get; set; }

        public short MaxRetry { get; set; }
    }
}
