using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Commands;

public record UpdateCuratedMemberResponse(bool Success, CuratedMember? UpdatedCuratedMember = null);