using MassTransit.Mediator;

namespace Pi.GlobalMarketData.Application.Queries;

public record GetByIdCuratedFilterRequest(string id) : Request<GetByIdCuratedFilterResponse>;