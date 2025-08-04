namespace Pi.SetService.Application.Models;

public record SblInstrumentSyncInfo
{
    public required string Symbol { get; init; }
    public required decimal InterestRate { get; init; }
    public required decimal RetailLender { get; init; }
}
