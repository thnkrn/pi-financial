using MassTransit.Mediator;

namespace Pi.GlobalMarketData.Application.Queries;

public record GetCuratedMemberRequest(int? curatedListId) : Request<GetCuratedMemberResponse>;