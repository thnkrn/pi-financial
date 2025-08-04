using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Queries;

public record GetCuratedListResponse(List<CuratedList> Data);
