namespace Pi.OnePort.Db2.Models;

public abstract class RootOrder
{
    public required ulong OrderNo { get; set; }
    public string? AccountNo { get; set; }
    public string? SecSymbol { get; set; }
    public string? TrusteeId { get; set; }
    public string? Side { get; set; }
    public decimal? Price { get; set; }
    public decimal? Volume { get; set; }
    public decimal? PubVolume { get; set; }
    public string? Condition { get; set; }
    public string? ConditionPrice { get; set; }
    public string? OrderType { get; set; }
    public string? EnterId { get; set; }
    public string? OrderDate { get; set; }
    public string? OrderTime { get; set; }
    public decimal? MatchVolume { get; set; }
    public string? CancelId { get; set; }
    public string? CancelTime { get; set; }
    public decimal? CancelVolume { get; set; }
    public string? ServiceType { get; set; }
}
