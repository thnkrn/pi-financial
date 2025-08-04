using Pi.MarketData.Domain.Models.Response.MarketDataManagement;

namespace Pi.SetMarketData.Application.Queries;

public record GetCuratedFilterResponse(List<CuratedFilterResponse> Data);