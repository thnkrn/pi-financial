namespace Pi.WalletService.Application.Services.GlobalEquities;

public record Position(
    string Currency,
    decimal Quantity,
    decimal AveragePrice,
    decimal ConvertedPnl,
    decimal ConvertedValue
);

public record AccountSummaryResponse(string AccountId, string Currency, long Timestamp, string AvailableBalance,
    List<Position> PositionsList);