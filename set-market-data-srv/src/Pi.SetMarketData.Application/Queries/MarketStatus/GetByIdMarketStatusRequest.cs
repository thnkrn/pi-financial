using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.MarketStatus;

public record GetByIdMarketStatusRequest(string id) : Request<GetByIdMarketStatusResponse>;