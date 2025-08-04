using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries;

public record GetByIdCuratedListRequest(string id) : Request<GetByIdCuratedListResponse>;