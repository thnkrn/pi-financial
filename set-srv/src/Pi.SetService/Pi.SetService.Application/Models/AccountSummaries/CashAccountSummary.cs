namespace Pi.SetService.Application.Models.AccountSummaries;

public record CashAccountSummary() : AccountSummary
{
    public required decimal Ar { get; init; }
    public required decimal Ap { get; init; }
    public required decimal ArTrade { get; init; }
    public required decimal ApTrade { get; init; }
    public required decimal TotalBuyMatch { get; init; }
    public required decimal TotalBuyUnmatch { get; init; }
    public required BackofficeAvailableBalance? BackofficeAvailableBalance { get; init; }
    public string? CreditType { get; init; }

    public override decimal TotalValue =>
        CashBalance - Ar + Ap + TotalMarketValue - (BackofficeAvailableBalance?.ArTrade ?? 0)
        + (BackofficeAvailableBalance?.ApTrade ?? 0);
}
