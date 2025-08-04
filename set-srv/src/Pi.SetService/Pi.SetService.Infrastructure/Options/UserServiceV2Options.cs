using System.ComponentModel.DataAnnotations;

namespace Pi.SetService.Infrastructure.Options
{
    public class UserServiceV2Options
    {
        public const string Options = "UserV2";

        [Required]
        public string Host { get; set; } = string.Empty;

        public int TimeoutMS { get; set; }

        public short MaxRetry { get; set; }
    }
}
