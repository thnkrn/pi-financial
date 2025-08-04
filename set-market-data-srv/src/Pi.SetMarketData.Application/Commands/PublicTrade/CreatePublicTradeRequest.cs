using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.PublicTrade;

public record CreatePublicTradeRequest(Domain.Entities.PublicTrade PublicTrade) : Request<CreatePublicTradeResponse>;