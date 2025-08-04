namespace Pi.WalletService.Application.Options;

public class OnlineDirectDebitOptions
{
    public const string Options = "OnlineDirectDebit";
    public Dictionary<string, string> RegistrationCallbackUrl { get; set; } = new Dictionary<string, string>();
}