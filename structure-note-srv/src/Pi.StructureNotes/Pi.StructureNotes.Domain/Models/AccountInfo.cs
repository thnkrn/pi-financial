namespace Pi.StructureNotes.Domain.Models;

public record AccountInfo
{
    public string AccountId { get; init; }
    public string AccountNo { get; init; }
    public string CustCode { get; init; }
}
