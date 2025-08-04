using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries;

public record GetByIdCuratedFilterRequest(string id) : Request<GetByIdCuratedFilterResponse>;