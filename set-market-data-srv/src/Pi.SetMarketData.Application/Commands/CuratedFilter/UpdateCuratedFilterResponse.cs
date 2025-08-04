using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Commands;

public record UpdateCuratedFilterResponse(bool Success, CuratedFilter? UpdatedCuratedFilter = null);