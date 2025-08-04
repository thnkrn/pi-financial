using System.ComponentModel.DataAnnotations;

namespace Pi.WalletService.Infrastructure.Options
{
    public class CgsBankServiceOptions
    {
        public const string Options = "CgsBank";

        [Required]
        public string Host { get; set; } = string.Empty;

        public int TimeoutMS { get; set; }

        public short MaxRetry { get; set; }

        [Required]
        public string ApiKey { get; set; } = string.Empty;

        [Required]
        public string SecretKey { get; set; } = string.Empty;
    }
}
