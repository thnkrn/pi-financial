namespace Pi.GlobalEquities.DomainModels;

public class OrderUpdates : IOrderUpdates
{
    public string ProviderId { get; init; }
    public decimal Quantity { get; init; }
    public decimal? LimitPrice { get; init; }
    public decimal? StopPrice { get; init; }

    public OrderStatus Status { get; init; }
    public IEnumerable<OrderFill> Fills { get; init; }
    public ProviderInfo ProviderInfo { get; init; }
}
