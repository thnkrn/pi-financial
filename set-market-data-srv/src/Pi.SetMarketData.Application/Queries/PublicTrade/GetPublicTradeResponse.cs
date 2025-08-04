namespace Pi.SetMarketData.Application.Queries.PublicTrade;

public record GetPublicTradeResponse(List<Domain.Entities.PublicTrade> Data);