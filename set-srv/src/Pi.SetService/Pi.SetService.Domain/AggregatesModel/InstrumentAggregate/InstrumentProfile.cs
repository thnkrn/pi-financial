namespace Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

public record InstrumentProfile
{
    public required string Symbol { get; init; }
    public required string Logo { get; init; }
    public required string FriendlyName { get; init; }
    public required string InstrumentCategory { get; init; }
}