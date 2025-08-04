using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Commands;

public record UpdateCuratedListResponse(bool Success, CuratedList? UpdatedCuratedList = null);