namespace Pi.SetMarketData.Application.Queries.MarketStatus;

public record GetMarketStatusResponse(List<Domain.Entities.MarketStatus> Data);