namespace Pi.SetService.API.Models;

public record SetAccountBalanceInfoResponse
{
    public required string AccountNo { get; init; }
    public required decimal BuyLimit { get; init; }
    public required decimal CreditLimit { get; init; }
    public required decimal CashBalance { get; init; }
}

public record SetAccountCashBalanceInfoResponse : SetAccountBalanceInfoResponse
{
    public required decimal PendingSettlement { get; init; }
}

public record SetAccountCreditBalanceInfoResponse : SetAccountBalanceInfoResponse
{
    public required decimal ExcessEquity { get; init; }
    public required decimal MarginRequired { get; init; }
    public required decimal MarginRatio { get; init; }
    public required decimal Liabilities { get; init; }
    public required decimal CallForce { get; init; }
    public required decimal MarginCall { get; init; }
}