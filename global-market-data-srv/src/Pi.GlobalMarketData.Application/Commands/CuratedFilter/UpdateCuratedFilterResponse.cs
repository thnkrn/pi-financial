using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Commands;

public record UpdateCuratedFilterResponse(bool Success, CuratedFilter? UpdatedCuratedFilter = null);