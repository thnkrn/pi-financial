namespace Pi.SetService.Application.Models;

public record CancelOfflineOrder
{
    public required DateTime OrderDateTime { get; init; }
    public required long BrokerOrderId { get; init; }
    public required string CancelId { get; init; }
    public required DateTime CancelDateTime { get; init; }
    public required bool DelFlag { get; init; }
}
