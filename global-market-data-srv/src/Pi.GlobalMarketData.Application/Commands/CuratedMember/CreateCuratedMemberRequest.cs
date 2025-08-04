using MassTransit.Mediator;
using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Commands;

public record CreateCuratedMemberRequest(IEnumerable<CuratedMember> CuratedMember) : Request<CreateCuratedMemberResponse>;