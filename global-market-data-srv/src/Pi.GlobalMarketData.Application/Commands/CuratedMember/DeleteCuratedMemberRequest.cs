using MassTransit.Mediator;

namespace Pi.GlobalMarketData.Application.Commands;

public record DeleteCuratedMemberRequest(string id) : Request<DeleteCuratedMemberResponse>;