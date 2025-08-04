using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Commands;

public record CreateCuratedFilterResponse(bool Success, IEnumerable<CuratedFilter> CreatedCuratedFilter);