namespace Pi.SetMarketData.Application.Commands.MarketStatus;

public record UpdateMarketStatusResponse(bool Success, Domain.Entities.MarketStatus? UpdatedMarketStatus = null);