namespace Pi.WalletService.Application.Options;

public class EmailOptions
{
    public const string Options = "Emails";
    public required string RequestAtsEmailRecipients { get; set; }
}