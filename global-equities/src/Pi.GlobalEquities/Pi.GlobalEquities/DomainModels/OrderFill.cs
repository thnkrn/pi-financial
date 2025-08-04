namespace Pi.GlobalEquities.DomainModels;

public readonly record struct OrderFill(DateTime Timestamp, decimal Quantity, decimal Price)
{
    public decimal TotalPrice => Price * Quantity;
}
