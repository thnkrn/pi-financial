namespace Pi.SetService.Application.Models.AccountInfo;

public record CashAccountInfo : AccountInfo
{
    public required decimal Ar { get; init; }
    public required decimal Ap { get; init; }
    public required decimal ArTrade { get; init; }
    public required decimal ApTrade { get; init; }
    public required decimal TotalBuyMatch { get; init; }
    public required decimal TotalBuyUnmatch { get; init; }
    public required BackofficeAvailableBalance? BackofficeAvailableBalance { get; init; }
    public string? CreditType { get; init; }

    public decimal PendingSettlement =>
        (Ap + BackofficeAvailableBalance?.ApTrade ?? 0) -
        (Ar + BackofficeAvailableBalance?.ArTrade ?? 0);
}
