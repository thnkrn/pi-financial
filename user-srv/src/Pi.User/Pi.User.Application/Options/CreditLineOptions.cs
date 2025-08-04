namespace Pi.User.Application.Options;

public class CreditLineOptions
{
    public const string Options = "FixedCreditLine";
    public decimal GlobalEquity { get; set; } = 30000;
}
