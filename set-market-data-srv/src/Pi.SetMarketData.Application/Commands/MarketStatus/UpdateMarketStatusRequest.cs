using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.MarketStatus;

public record UpdateMarketStatusRequest(string id, Domain.Entities.MarketStatus MarketStatus) : Request<UpdateMarketStatusResponse>;