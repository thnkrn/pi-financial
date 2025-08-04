namespace Pi.SetService.IntegrationEvents;

public record SetOrderPlaced
{
    public required Guid CorrelationId { get; init; }
    public required string OrderNo { get; init; }
    public required string TradingAccountNo { get; init; }
}
