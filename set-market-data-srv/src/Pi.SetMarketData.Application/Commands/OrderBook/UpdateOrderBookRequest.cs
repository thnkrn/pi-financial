using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.OrderBook;

public record UpdateOrderBookRequest(string id, Domain.Entities.OrderBook OrderBook) : Request<UpdateOrderBookResponse>;