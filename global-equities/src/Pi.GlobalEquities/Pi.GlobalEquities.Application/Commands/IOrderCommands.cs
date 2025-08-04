using Pi.GlobalEquities.Application.Models.Dto;
using Pi.GlobalEquities.DomainModels;

namespace Pi.GlobalEquities.Application.Commands;

public interface IOrderCommands
{
    Task<OrderDto> PlaceOrder(string userId, IOrder request, CancellationToken ct = default);
    Task<OrderDto> ModifyOrder(string userId, string refId, IOrderValues request,
        CancellationToken ct = default);
    Task<OrderDto> CancelOrder(string userId, string refId, CancellationToken ct = default);
}
