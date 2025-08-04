namespace Pi.SetService.Application.Models;

public record CeilingFloor
{
    public required string Symbol { get; init; }
    public required decimal Floor { get; init; }
    public required decimal Ceiling { get; init; }
}
