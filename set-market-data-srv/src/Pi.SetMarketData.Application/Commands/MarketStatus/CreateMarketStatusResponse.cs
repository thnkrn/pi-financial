namespace Pi.SetMarketData.Application.Commands.MarketStatus;

public record CreateMarketStatusResponse(bool Success, Domain.Entities.MarketStatus? CreatedMarketStatus = null);