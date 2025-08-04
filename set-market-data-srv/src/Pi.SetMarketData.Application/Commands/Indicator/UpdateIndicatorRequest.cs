using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Indicator;

public record UpdateIndicatorRequest(string id, Domain.Entities.Indicator Indicator) : Request<UpdateIndicatorResponse>;