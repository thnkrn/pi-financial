using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.PublicTrade;

public record GetPublicTradeRequest : Request<GetPublicTradeResponse>;