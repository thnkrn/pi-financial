using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.MarketStatus;

public record CreateMarketStatusRequest(Domain.Entities.MarketStatus MarketStatus) : Request<CreateMarketStatusResponse>;