using MassTransit.Mediator;
using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Commands;

public record UpdateCuratedMemberRequest(string id, CuratedMember CuratedMember) : Request<UpdateCuratedMemberResponse>;