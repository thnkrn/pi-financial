using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Commands;

public record CreateCuratedListResponse(bool Success, IEnumerable<CuratedList> CreatedCuratedList);