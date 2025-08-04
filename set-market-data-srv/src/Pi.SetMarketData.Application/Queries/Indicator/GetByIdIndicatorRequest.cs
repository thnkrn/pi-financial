using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.Indicator;

public record GetByIdIndicatorRequest(string id) : Request<GetByIdIndicatorResponse>;