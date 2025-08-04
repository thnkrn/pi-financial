namespace Pi.WalletService.Application.Options;

public class QrCodeOptions
{
    public const string Options = "QrCode";
    public int TimeOutMinute { get; set; } = 15;
}