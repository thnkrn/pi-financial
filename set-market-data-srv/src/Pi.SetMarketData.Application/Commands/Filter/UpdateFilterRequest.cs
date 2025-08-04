using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Filter;

public record UpdateFilterRequest(string id, Domain.Entities.Filter Filter) : Request<UpdateFilterResponse>;