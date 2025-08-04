namespace Pi.SetService.Application.Models;

public record BackofficeAvailableBalance
{
    public required string TradingAccountNo { get; init; }
    public required string AccountNo { get; init; }
    public required decimal CashBalance { get; init; }
    public required decimal ArTrade { get; init; }
    public required decimal ApTrade { get; init; }
    public required decimal MarketValue { get; init; }
    public required DateTime PostDateTime { get; init; }
}
