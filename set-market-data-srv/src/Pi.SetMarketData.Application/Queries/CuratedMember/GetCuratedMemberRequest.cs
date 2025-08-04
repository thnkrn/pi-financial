using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries;

public record GetCuratedMemberRequest(int? curatedListId) : Request<GetCuratedMemberResponse>;