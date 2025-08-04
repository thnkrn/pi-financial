using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands;

public record DeleteCuratedFilterRequest(string id) : Request<DeleteCuratedFilterResponse>;