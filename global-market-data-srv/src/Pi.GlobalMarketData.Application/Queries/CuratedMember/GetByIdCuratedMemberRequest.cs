using MassTransit.Mediator;

namespace Pi.GlobalMarketData.Application.Queries;

public record GetByIdCuratedMemberRequest(string id) : Request<GetByIdCuratedMemberResponse>;