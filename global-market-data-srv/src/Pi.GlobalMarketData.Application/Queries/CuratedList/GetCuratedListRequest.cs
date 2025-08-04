using MassTransit.Mediator;

namespace Pi.GlobalMarketData.Application.Queries;

public record GetCuratedListRequest : Request<GetCuratedListResponse>;
