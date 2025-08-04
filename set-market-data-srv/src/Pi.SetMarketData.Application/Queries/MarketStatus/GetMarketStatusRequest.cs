using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.MarketStatus;

public record GetMarketStatusRequest : Request<GetMarketStatusResponse>;