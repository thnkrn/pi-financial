using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands;

public record DeleteCuratedMemberRequest(string id) : Request<DeleteCuratedMemberResponse>;