using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Commands;

public record CreateCuratedMemberRequest(IEnumerable<CuratedMember> CuratedMember) : Request<CreateCuratedMemberResponse>;