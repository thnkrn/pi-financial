using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Commands;

public record UpdateCuratedMemberResponse(bool Success, CuratedMember? UpdatedCuratedMember = null);