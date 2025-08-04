using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Commands;

public record UpdateCuratedListResponse(bool Success, CuratedList? UpdatedCuratedList = null);