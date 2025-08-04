namespace Pi.WalletService.Application.Options;

public class FeaturesOptions
{
    public const string Options = "Features";
    public bool ShouldGetBankAccountFromOnboardService { get; set; }
    public List<string> AllowDepositWithdrawV2CustCode { get; set; } = new();
    public string FreewillOpeningTime { get; set; } = "08:30";
    public string FreewillClosingTime { get; set; } = "16:30";
    public string KkpOpeningTime { get; set; } = "00:10";
    public string KkpClosingTime { get; set; } = "23:55";
    public bool ShouldUseNewErrorCodeOnBankMaintenance { get; set; }

    /// <summary>
    /// List of bank code to be forwarded to DepositODD
    /// Applied when channel is ATS or ODD
    /// </summary>
    public List<string> OddDepositBankCode { get; set; } = new();
}