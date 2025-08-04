namespace Pi.SetService.Application.Models.AccountInfo;

public abstract record AccountInfo
{
    public required string TradingAccountNo { get; init; }
    public required string AccountNo { get; init; }
    public required string TraderId { get; init; }
    public required decimal CreditLimit { get; init; }
    public required decimal BuyCredit { get; init; }
    public required decimal CashBalance { get; init; }
}