using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Queries;

public record GetCuratedListResponse(List<CuratedList> Data);