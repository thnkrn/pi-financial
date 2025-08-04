using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.PublicTrade;

public record UpdatePublicTradeRequest(string id, Domain.Entities.PublicTrade PublicTrade) : Request<UpdatePublicTradeResponse>;