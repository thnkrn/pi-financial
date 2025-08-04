using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.PublicTrade;

public record DeletePublicTradeRequest(string id) : Request<DeletePublicTradeResponse>;