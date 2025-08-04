using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Commands;

public record CreateCuratedMemberResponse(bool Success, IEnumerable<CuratedMember> CreatedCuratedMember);