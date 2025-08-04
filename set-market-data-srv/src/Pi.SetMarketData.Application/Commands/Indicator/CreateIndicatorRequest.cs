using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Indicator;

public record CreateIndicatorRequest(Domain.Entities.Indicator Indicator) : Request<CreateIndicatorResponse>;