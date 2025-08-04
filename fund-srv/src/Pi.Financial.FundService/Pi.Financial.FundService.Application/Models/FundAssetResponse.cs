namespace Pi.Financial.FundService.Application.Models;

public record FundAssetResponse
{
    public required string UnitholderId { get; init; }
    public required string FundCode { get; init; }
    public required decimal Unit { get; init; }
    public required decimal Amount { get; init; }
    public required decimal RemainUnit { get; init; }
    public required decimal RemainAmount { get; init; }
    public required decimal PendingAmount { get; init; }
    public required decimal PendingUnit { get; init; }
    public required decimal AvgCost { get; init; }
    public required decimal Nav { get; init; }
    public required DateOnly NavDate { get; init; }
}

public record BoFundAssetResponse : FundAssetResponse
{
    public required string AmcCode { get; init; }
    public required string AccountId { get; init; }
}
