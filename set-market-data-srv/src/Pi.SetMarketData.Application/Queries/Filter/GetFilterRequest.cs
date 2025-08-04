using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.Filter;

public record GetFilterRequest : Request<GetFilterResponse>;