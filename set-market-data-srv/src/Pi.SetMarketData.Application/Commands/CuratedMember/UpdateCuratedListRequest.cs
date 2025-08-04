using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Commands;

public record UpdateCuratedMemberRequest(string id, CuratedMember CuratedMember) : Request<UpdateCuratedMemberResponse>;