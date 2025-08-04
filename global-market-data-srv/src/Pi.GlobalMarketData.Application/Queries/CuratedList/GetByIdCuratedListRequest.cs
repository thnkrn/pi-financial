using MassTransit.Mediator;

namespace Pi.GlobalMarketData.Application.Queries;

public record GetByIdCuratedListRequest(string id) : Request<GetByIdCuratedListResponse>;
