using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.PublicTrade;

public record GetByIdPublicTradeRequest(string id) : Request<GetByIdPublicTradeResponse>;