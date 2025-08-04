namespace Pi.GlobalEquities.DomainModels;

public interface IOrderStatus
{
    string ProviderId { get; }
    OrderStatus Status { get; }
    IEnumerable<OrderFill> Fills { get; }
    ProviderInfo ProviderInfo { get; }
}
