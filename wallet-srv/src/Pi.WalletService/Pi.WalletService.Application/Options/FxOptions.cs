namespace Pi.WalletService.Application.Options;

public class FxOptions
{
    public const string Options = "Fx";
    public decimal DepositMarkUpRate { get; set; }
    public decimal WithdrawalMarkUpRate { get; set; }
}