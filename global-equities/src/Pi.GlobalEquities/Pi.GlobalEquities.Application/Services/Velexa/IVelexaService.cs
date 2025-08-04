using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.DomainModels.MarketData;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Services.Velexa;

public interface IVelexaService
{
    Task<IOrderStatus> PlaceOrder(IOrder order, string providerAccountId, CancellationToken ct);
    Task<IOrderUpdates> UpdateOrder(string orderId, IOrderValues values, CancellationToken ct);
    Task<IOrderStatus> CancelOrder(string orderId, CancellationToken ct);
    Task<IOrder> GetOrder(string orderId, CancellationToken ct);
}

