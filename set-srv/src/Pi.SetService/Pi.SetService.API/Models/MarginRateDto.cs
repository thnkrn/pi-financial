namespace Pi.SetService.API.Models;

public record MarginRateResponse
{
    public required string Symbol { get; init; }
    public required decimal Rate { get; init; }
    public required bool IsTurnoverList { get; init; }
}