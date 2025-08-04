using System.ComponentModel.DataAnnotations;

namespace Pi.Financial.FundService.Infrastructure.Options
{
    public class UserServiceOptions
    {
        public const string Options = "User";

        [Required]
        public string Host { get; set; } = string.Empty;

        public int TimeoutMS { get; set; }

        public short MaxRetry { get; set; }
    }
}
