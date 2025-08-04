namespace Pi.SetService.API.Models;

public record AccountInstrumentAvailableBalanceRequest
{
    public required string Symbol { get; init; }
    public required string TradingAccountNo { get; init; }
}

public record AccountSblInstrumentAvailableBalanceResponse
{
    public required decimal Ee { get; init; }
    public required int ShelfAvailable { get; init; }
    public required bool SblFlag { get; init; }
    public required decimal BorrowCredit { get; init; }
    public required decimal MaximumShares { get; init; }
    public required decimal ShortUnitAvailable { get; init; }
    public required decimal Power { get; init; }
    public required decimal InitialMarginRate { get; init; }
    public required decimal ClosePriceYesterday { get; init; }
    public required bool AllowBorrowing { get; init; }
}

public record AccountInstrumentAvailableBalanceResponse
{
    public required AccountDetail AccountDetail { get; init; }
    public required UnitRemain UnitRemain { get; init; }
}

public record AccountDetail
{
    public required string BalanceUnit { get; init; }
    public required decimal Balance { get; init; }
}

public record UnitRemain
{
    public required string AssetSymbol { get; init; }
    public required decimal Unit { get; init; }
    public required decimal NvdrUnit { get; init; }
    public required decimal ShortUnit { get; init; }
    public required decimal ShortNvdrUnit { get; init; }
    public decimal Amount { get; init; } // Note: Add this key for supported app
}
