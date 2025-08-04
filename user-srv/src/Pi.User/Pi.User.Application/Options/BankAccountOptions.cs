using System.ComponentModel.DataAnnotations;

namespace Pi.User.Application.Options
{
    public class BankAccountOptions
    {
        public const string Options = "BankAccount";

        [Required]
        public Dictionary<string, string> BankCodes { get; set; } = new Dictionary<string, string>();
    }
}

