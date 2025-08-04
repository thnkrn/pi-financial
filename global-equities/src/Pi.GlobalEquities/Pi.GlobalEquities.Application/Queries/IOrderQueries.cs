using Pi.GlobalEquities.Application.Models;
using Pi.GlobalEquities.Application.Models.Dto;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Queries;

public interface IOrderQueries
{
    Task<IEnumerable<OrderDto>> GetActiveOrders(string userId, string accountId, CancellationToken ct = default);
    Task<IEnumerable<OrderDto>> GetOrders(string userId, string accountId, DateTime from, DateTime to, OrderSide? side,
        bool? hasFilled, CancellationToken ct = default);
    Task<Position?> GetPosition(string account, string symbolId, CancellationToken ct = default);
}
