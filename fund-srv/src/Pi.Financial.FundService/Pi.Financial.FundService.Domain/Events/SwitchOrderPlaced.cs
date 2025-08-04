namespace Pi.Financial.FundService.Domain.Events;

public record SwitchOrderPlaced
{
    public required Guid CorrelationId { get; init; }
    public required string OrderNo { get; init; }
}
