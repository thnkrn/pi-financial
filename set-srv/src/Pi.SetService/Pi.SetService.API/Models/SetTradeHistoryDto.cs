namespace Pi.SetService.API.Models;

public record SetTradeHistoryResponse
{
    public required string AccountCode { get; init; }
    public required string InstrumentSymbol { get; init; }
    public required string Currency { get; init; }
    public required string Side { get; init; }
    public required string TradeDate { get; init; }
    public required string TradeTime { get; init; }
    public required decimal Price { get; init; }
    public required decimal Volume { get; init; }
    public required decimal TotalAmount { get; init; }
    public required decimal Amount { get; init; }
    public required decimal TotalCommissionAndVat { get; init; }
}