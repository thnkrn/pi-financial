using System.ComponentModel.DataAnnotations;
namespace Pi.WalletService.Infrastructure.Options;

public class PaymentServiceOptions
{
    public const string Options = "Payment";
    [Required]
    public string Host { get; set; } = string.Empty;

    [Required]
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutMS { get; set; }

    public short MaxRetry { get; set; }

}
