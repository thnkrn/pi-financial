using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.Indicator;

public record GetIndicatorRequest : Request<GetIndicatorResponse>;