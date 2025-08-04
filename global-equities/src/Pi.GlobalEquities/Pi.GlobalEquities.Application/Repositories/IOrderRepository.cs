using Pi.GlobalEquities.DomainModels;

namespace Pi.GlobalEquities.Application.Repositories;

public interface IOrderRepository
{
    Task UpdateOrder(string orderId, IOrder order, CancellationToken ct);
    Task<IEnumerable<IOrder>?> GetOrders(string refId, CancellationToken ct);
}
