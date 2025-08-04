using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.OrderBook;

public record CreateOrderBookRequest(Domain.Entities.OrderBook OrderBook) : Request<CreateOrderBookResponse>;