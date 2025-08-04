using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries;

public record GetCuratedListRequest : Request<GetCuratedListResponse>;