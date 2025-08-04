using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Commands;

public record CreateCuratedMemberResponse(bool Success, IEnumerable<CuratedMember> CreatedCuratedMember);