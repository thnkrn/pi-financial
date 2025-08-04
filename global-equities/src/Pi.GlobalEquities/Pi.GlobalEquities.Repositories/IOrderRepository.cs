using MongoDB.Driver;
using Pi.GlobalEquities.DomainModels;

namespace Pi.GlobalEquities.Repositories;

public interface IOrderRepository
{
    Task CreateOrder(IOrder order, CancellationToken ct);
    Task UpdateOrder(string orderId, IOrder order, CancellationToken ct);
    Task<IOrder> GetOrder(string userId, string orderId, CancellationToken ct);
}
