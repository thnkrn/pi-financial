using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Commands;

public record CreateCuratedListResponse(bool Success, IEnumerable<CuratedList> CreatedCuratedList);