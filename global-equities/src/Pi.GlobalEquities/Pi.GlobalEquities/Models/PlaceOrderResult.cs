namespace Pi.GlobalEquities.Models;

public class PlaceOrderResult : IOrderStatus
{
    public string ProviderId => ProviderInfo.OrderId;
    public OrderStatus Status { get; init; }
    public ProviderInfo ProviderInfo { get; init; }
    public IEnumerable<OrderFill> Fills { get; init; }
}
