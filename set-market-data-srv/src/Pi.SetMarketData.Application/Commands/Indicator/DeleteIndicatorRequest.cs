using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Indicator;

public record DeleteIndicatorRequest(string id) : Request<DeleteIndicatorResponse>;