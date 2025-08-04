using MassTransit.Mediator;

namespace Pi.GlobalMarketData.Application.Commands;

public record DeleteCuratedFilterRequest(string id) : Request<DeleteCuratedFilterResponse>;