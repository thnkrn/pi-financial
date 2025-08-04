using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.Filter;

public record GetByIdFilterRequest(string id) : Request<GetByIdFilterResponse>;