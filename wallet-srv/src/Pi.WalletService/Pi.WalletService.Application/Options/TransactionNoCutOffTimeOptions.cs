namespace Pi.WalletService.Application.Options;

public class TransactionNoCutOffTimeOptions
{
    public const string Options = "TransactionNoCutOffTime";
    public string NonGe { get; set; } = "00:00";
    public string Ge { get; set; } = "05:01";
}