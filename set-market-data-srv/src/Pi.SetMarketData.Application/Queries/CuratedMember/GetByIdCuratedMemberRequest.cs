using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries;

public record GetByIdCuratedMemberRequest(string id) : Request<GetByIdCuratedMemberResponse>;