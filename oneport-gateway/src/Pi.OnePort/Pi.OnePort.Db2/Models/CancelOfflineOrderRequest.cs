namespace Pi.OnePort.Db2.Models;

public record CancelOfflineOrderRequest
{
    public required DateTime OrderDateTime { get; init; } // default = now
    public required ulong OrderNo { get; init; }
    public required string CancelId { get; init; }
    public required DateTime CancelDateTime { get; init; } // default = now
    public required bool DelFlag { get; init; }
}
