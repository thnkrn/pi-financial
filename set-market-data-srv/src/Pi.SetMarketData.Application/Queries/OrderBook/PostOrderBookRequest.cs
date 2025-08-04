using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries.OrderBook;

public record PostOrderBookRequest(MarketOrderBookRequest Data) : Request<PostOrderBookResponse>;
