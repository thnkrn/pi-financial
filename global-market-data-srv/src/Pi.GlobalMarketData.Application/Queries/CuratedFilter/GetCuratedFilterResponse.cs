using Pi.GlobalMarketData.Domain.Models.Response.MarketDataManagement;

namespace Pi.GlobalMarketData.Application.Queries;

public record GetCuratedFilterResponse(IEnumerable<CuratedFilterResponse> Data);