using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.OrderBook;

public record GetOrderBookRequest : Request<GetOrderBookResponse>;