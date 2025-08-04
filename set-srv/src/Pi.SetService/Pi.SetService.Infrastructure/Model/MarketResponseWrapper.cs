namespace Pi.SetService.Infrastructure.Model;

public record MarketResponseWrapper<T>
{
    public required T? Response { get; init; }
    public required string Symbol { get; init; }
}
