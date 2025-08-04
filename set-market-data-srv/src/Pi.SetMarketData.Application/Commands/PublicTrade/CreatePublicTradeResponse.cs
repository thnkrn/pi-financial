namespace Pi.SetMarketData.Application.Commands.PublicTrade;

public record CreatePublicTradeResponse(bool Success, Domain.Entities.PublicTrade? CreatedPublicTrade = null);