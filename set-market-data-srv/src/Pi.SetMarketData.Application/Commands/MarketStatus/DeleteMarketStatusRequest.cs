using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.MarketStatus;

public record DeleteMarketStatusRequest(string id) : Request<DeleteMarketStatusResponse>;