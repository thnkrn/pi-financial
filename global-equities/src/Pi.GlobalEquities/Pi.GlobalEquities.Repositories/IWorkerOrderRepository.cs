using Pi.GlobalEquities.DomainModels;

namespace Pi.GlobalEquities.Repositories;

public interface IWorkerOrderRepository : IOrderRepository
{
    Task<IOrder> GetOrder(string orderId, CancellationToken ct);
}
