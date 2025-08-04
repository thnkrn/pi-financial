using System.ComponentModel.DataAnnotations;

namespace Pi.SetService.Infrastructure.Options
{
    public class MarketServiceOptions
    {
        public const string Options = "Market";

        [Required]
        public string Host { get; set; } = string.Empty;

        public int TimeoutMS { get; set; }

        public short MaxRetry { get; set; }
    }
}
