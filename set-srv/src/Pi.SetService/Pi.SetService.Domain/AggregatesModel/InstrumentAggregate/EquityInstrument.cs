namespace Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

public record EquityInstrument
{
    public required string Symbol { get; init; }
    public required bool IsNew { get; init; }
    public required InstrumentProfile Profile { get; init; }
    public required TradingDetail TradingDetail { get; init; }
    public IEnumerable<CorporateAction>? CorporateActions { get; init; } = [];
    public IEnumerable<TradingSign>? TradingSigns { get; init; } = [];
    public MarketDetail? MarketDetail { get; init; }
}
