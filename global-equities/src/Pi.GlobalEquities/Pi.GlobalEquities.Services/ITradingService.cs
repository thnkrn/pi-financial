namespace Pi.GlobalEquities.Services;

public interface ITradingService : ITradingReadService
{
    Task<IOrderStatus> PlaceOrder(IOrder order, string providerAccountId, CancellationToken ct);
    Task<IOrderUpdates> UpdateOrder(string orderId, IOrderValues values, CancellationToken ct);
    Task<IOrderStatus> CancelOrder(string orderId, CancellationToken ct);
}
