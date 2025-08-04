namespace Pi.GlobalEquities.Models;

public class TransactionItem
{
    public string Id { get; init; }
    public string ParentId { get; init; }
    public string OrderId { get; init; }
    public string Venue { get; init; }
    public string Symbol { get; init; }
    public string SymbolId
    {
        get
        {
            return !string.IsNullOrWhiteSpace(Venue) && !string.IsNullOrWhiteSpace(Symbol)
                ? $"{Symbol}.{Venue}"
                : null;
        }
    }

    public string Asset { get; init; }
    public decimal Value { get; init; }
    public OperationType OperationType { get; init; }
    public long Timestamp { get; init; }

    public IList<TransactionItem> Children { get; init; } = new List<TransactionItem>();
}
