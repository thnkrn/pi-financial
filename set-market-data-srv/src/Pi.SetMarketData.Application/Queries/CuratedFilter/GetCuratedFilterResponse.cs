using Pi.SetMarketData.Domain.Models.Response.MarketDataManagement;

namespace Pi.SetMarketData.Application.Queries;

public record GetCuratedFilterResponse(IEnumerable<CuratedFilterResponse> Data);