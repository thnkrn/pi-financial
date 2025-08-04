namespace Pi.SetService.Application.Models;

public record CancelOrder
{
    public required string OrderNo { get; init; }
    public required string BrokerOrderId { get; init; }
    public required string EnterId { get; init; }
}

