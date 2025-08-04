namespace Pi.SetMarketData.Application.Commands.PublicTrade;

public record UpdatePublicTradeResponse(bool Success, Domain.Entities.PublicTrade? UpdatedPublicTrade = null);