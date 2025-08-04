namespace Pi.SetService.Domain.AggregatesModel.AccountAggregate;

public record AvailableCashBalance : AvailableBalance
{
    public required decimal Ar { get; init; }
    public required decimal Ap { get; init; }
    public required decimal ArTrade { get; init; }
    public required decimal ApTrade { get; init; }
    public required decimal TotalBuyMatch { get; init; }
    public required decimal TotalBuyUnmatch { get; init; }
    public string? CreditType { get; init; }
}