namespace Pi.SetMarketData.Application.Commands.TradingSign;

public record UpdateTradingSignResponse(bool Success, Domain.Entities.TradingSign? UpdatedTradingSign = null);