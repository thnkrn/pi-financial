using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.OrderBook;

public record DeleteOrderBookRequest(string id) : Request<DeleteOrderBookResponse>;