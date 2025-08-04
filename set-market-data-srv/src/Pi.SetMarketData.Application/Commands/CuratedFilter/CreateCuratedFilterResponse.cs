using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Commands;

public record CreateCuratedFilterResponse(bool Success, IEnumerable<CuratedFilter> CreatedCuratedFilter);