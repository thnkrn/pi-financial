using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Filter;

public record CreateFilterRequest(Domain.Entities.Filter Filter) : Request<CreateFilterResponse>;