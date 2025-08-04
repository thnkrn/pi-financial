namespace Pi.StructureNotes.Domain.Models;

public class Note : AssetBase
{
    public required string Isin { get; init; }
    public required string Type { get; init; }
    public required decimal? Yield { get; init; }
    public required decimal? Rebate { get; init; }
    public required string Underlying { get; init; }
    public required int? Tenors { get; init; }
    public required DateTime TradeDate { get; init; }
    public required DateTime IssueDate { get; init; }
    public required DateTime ValuationDate { get; init; }
    public required string Issuer { get; init; }
}
