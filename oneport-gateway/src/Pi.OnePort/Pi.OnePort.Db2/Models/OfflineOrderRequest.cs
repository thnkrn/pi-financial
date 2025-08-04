namespace Pi.OnePort.Db2.Models;

public record OfflineOrderRequest
{
    public required DateTime OrderDateTime { get; init; } // default = now
    public required ulong OrderNo { get; init; }
    public required string EnterId { get; init; } // default = {config}
    public required string SecSymbol { get; init; }
    public required string Side { get; init; }
    public required string AccountNo { get; init; }
    public required decimal Price { get; init; }
    public required decimal Volume { get; init; }
    public required decimal PubVolume { get; init; }
    public required string ServiceType { get; init; }
    public required string Market { get; init; }
    public string? Board { get; init; }
    public string? ConditionPrice { get; init; }
    public string? Condition { get; init; }
    public decimal BrokerNo { get; init; } = 0;
    public string? TrusteeId { get; init; }
    public string? OrderType { get; init; }
    public string? PutThoughFlag { get; init; }
    public string? Life { get; init; } // default = "D" (Day Order)
    public string? ExpireDate { get; init; } // default = "0000"
}
