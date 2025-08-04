namespace Pi.WalletService.Application.Options;

public class FeeOptions
{
    public const string Options = "Bank";
    public Kkp KKP { get; set; } = new Kkp();
    public GlobalFee Exante { get; set; } = new GlobalFee();
}

public class Kkp
{
    public string DepositFee { get; set; } = string.Empty;
    public string GlobalDepositFee { get; set; } = string.Empty;
    public string WithdrawFee { get; set; } = string.Empty;
    public string GlobalWithdrawFee { get; set; } = string.Empty;
}

public class GlobalFee
{
    public string DepositTransferFee { get; set; } = string.Empty;
    public string WithdrawTransferFee { get; set; } = string.Empty;
}
