namespace Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

public record CorporateAction
{
    public required DateOnly Date { get; init; }
    public required string CaType { get; init; }
}
