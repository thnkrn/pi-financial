using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Filter;

public record DeleteFilterRequest(string id) : Request<DeleteFilterResponse>;