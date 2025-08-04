using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.OrderBook;

public record GetByIdOrderBookRequest(string id) : Request<GetByIdOrderBookResponse>;